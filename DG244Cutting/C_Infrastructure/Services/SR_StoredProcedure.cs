using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Infrastructure;
using DG244Cutting.C_Infrastructure.Persistence.DIGIT_TRY.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DG244Cutting.C_Infrastructure.Services
{
    /// <summary>
    /// Service technique transverse d'exécution de procédures stockées SQL Server pour les
    /// traitements d'import et de synchronisation, résidant en <c>C_Infrastructure/Services/</c>
    /// et implémentant le contrat <see cref="IS_StoredProcedure"/> de
    /// <c>A_Domain/Interfaces/Services/Infrastructure/</c>.
    /// </summary>
    /// <remarks>
    /// <para>Contexte</para>
    /// <para>
    /// Ce Service est consommé par les UseCases orchestrateurs d'import MDB → SQL Server 2019
    /// lorsque certaines étapes du traitement sont implémentées sous forme de procédures
    /// stockées côté base. Il est résolu par injection de dépendances (portée Singleton
    /// conformément à P4-bis, §4.10.10 du 0230, en l'absence de dépendance scoped). Il consomme
    /// <c>IDbContextFactory&lt;DigitTryDbContext&gt;</c> au titre du second cas légitime du
    /// Pattern 3 du câblage triple du DbContext (§4.8.5 du 0230), tolérance expressément
    /// inscrite à la doctrine.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Exécuter des procédures stockées de manière sûre (paramétrée), contrôlée (whitelist)
    /// et traçable (CallChain propagée et enrichie au format normatif
    /// <c>{caller} &gt; {_callee} &gt; {nameof(method)}</c>).
    /// </para>
    /// <para>Responsabilités</para>
    /// <list type="bullet">
    /// <item><description>Valider les préconditions structurelles (nom de procédure whitelisté, argument cohérent par surcharge) en levant <see cref="Ex_Business"/> avec codes <c>BU_ER_01</c> / <c>BU_ER_02</c>.</description></item>
    /// <item><description>Honorer l'annulation coopérative via <c>CancellationToken</c> après validation et avant ouverture du contexte EF Core.</description></item>
    /// <item><description>Exécuter la procédure stockée via <c>ExecuteSqlRawAsync</c> en respectant la doctrine de paramétrage.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/> dans le quatrième catch terminal, avec enrichissement de la CallChain.</description></item>
    /// </list>
    /// <para>Non-responsabilités</para>
    /// <list type="bullet">
    /// <item><description>N'initie aucune transaction (pas de <c>BeginTransactionAsync</c>, <c>SaveChangesAsync</c>, <c>CommitAsync</c>, <c>RollbackAsync</c>) — la transaction relève exclusivement du UseCase orchestrateur.</description></item>
    /// <item><description>N'orchestre aucun scénario par appel d'un autre Service applicatif.</description></item>
    /// <item><description>N'appelle directement aucun Repository (<c>IR_*</c> ou <c>CR_*</c>).</description></item>
    /// <item><description>Ne journalise pas directement via <c>IS_ErrorLogger</c> et ne notifie pas directement via <c>IS_Notification</c>.</description></item>
    /// <item><description>N'exécute aucune procédure hors whitelist <c>_allowedProcedures</c> fournie par le Composition Root.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_StoredProcedure"/>
    /// <seealso cref="IS_ExClassifier"/>
    public class SR_StoredProcedure : IS_StoredProcedure
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du composant courant, résolu dynamiquement pour la construction de la CallChain
        /// au format normatif <c>{caller} &gt; {_callee} &gt; {nameof(method)}</c> (§4.5 du 0230, R-4.5.5).
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Factory EF Core fournissant un <see cref="DigitTryDbContext"/> de courte durée par appel,
        /// au titre du second cas légitime du Pattern 3 du câblage triple du DbContext (§4.8.5 du 0230).
        /// </summary>
        private readonly IDbContextFactory<DigitTryDbContext> _contextFactory;

        /// <summary>
        /// Service transversal d'utilité de requalification terminale des exceptions non prévues
        /// (§4.7.4 du 0230). Consommé exclusivement dans le quatrième catch terminal et dans le
        /// constructeur (garde de non-nullité).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        /// <summary>
        /// Whitelist des procédures stockées autorisées (anti-injection par nom de procédure).
        /// Fournie en dur par le Composition Root <c>SR_ConteneurDI</c> (configuration hors-périmètre
        /// du présent fil).
        /// </summary>
        private readonly ISet<string> _allowedProcedures;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_StoredProcedure"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>Contexte</para>
        /// <para>Instancié par injection de dépendances via <c>SR_ConteneurDI</c> en portée Singleton.</para>
        /// <para>Objectif</para>
        /// <para>Garantir la disponibilité du DbContextFactory, du service de requalification et du contrôle de sécurité (whitelist).</para>
        /// <para>Responsabilités</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c> à partir du type effectif (<c>GetType().Name</c>).</description></item>
        /// <item><description>Valider la non-nullité des dépendances injectées (garde <see cref="ArgumentNullException"/>).</description></item>
        /// </list>
        /// </remarks>
        /// <param name="contextFactory">Factory EF Core pour créer un <see cref="DigitTryDbContext"/> de courte durée par appel.</param>
        /// <param name="allowedProcedures">Whitelist des procédures stockées autorisées.</param>
        /// <param name="classifier">Service de classification terminale des exceptions non prévues.</param>
        /// <exception cref="ArgumentNullException">Levée si l'une des dépendances injectées est null.</exception>
        public SR_StoredProcedure(
            IDbContextFactory<DigitTryDbContext> contextFactory,
            ISet<string> allowedProcedures,
            IS_ExClassifier classifier)
        {
            _callee = GetType().Name;
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _allowedProcedures = allowedProcedures ?? throw new ArgumentNullException(nameof(allowedProcedures));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <inheritdoc />
        public async Task ExecuteAsync(string caller, string procedureName, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ValidateProcedureName(callChain, procedureName);

                ct.ThrowIfCancellationRequested();

                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                string sql = $"EXEC {procedureName};";
                await context.Database.ExecuteSqlRawAsync(sql, cancellationToken: ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                string callChainPlus = $"{callChain} > Stored procedure failed: {procedureName} > {DescribeSqlError(ex)}";

                throw _classifier.Execute(callChainPlus, ex);
            }
        }

        /// <inheritdoc />
        public async Task ExecuteArg1Async(string caller, string procedureName, object arg1, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteArg1Async)}";

            try
            {
                ValidateProcedureName(callChain, procedureName);

                if (arg1 is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "L'argument 'arg1' fourni à la procédure stockée est null.");

                ct.ThrowIfCancellationRequested();

                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                string sql = $"EXEC {procedureName} @p0;";
                await context.Database.ExecuteSqlRawAsync(sql, new object[] { arg1 }, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                string callChainPlus = $"{callChain} > Stored procedure failed: {procedureName} (arg1={arg1}) > {DescribeSqlError(ex)}";

                throw _classifier.Execute(callChainPlus, ex);
            }
        }

        /// <inheritdoc />
        public async Task ExecuteArg1StringAsync(string caller, string procedureName, string arg1, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteArg1StringAsync)}";

            try
            {
                ValidateProcedureName(callChain, procedureName);

                if (string.IsNullOrWhiteSpace(arg1))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "L'argument 'arg1' fourni à la procédure stockée est null ou vide.");

                ct.ThrowIfCancellationRequested();

                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                string sql = $"EXEC {procedureName} @p0;";
                await context.Database.ExecuteSqlRawAsync(sql, new object[] { arg1 }, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                string callChainPlus = $"{callChain} > Stored procedure failed: {procedureName} (arg1={arg1}) > {DescribeSqlError(ex)}";

                throw _classifier.Execute(callChainPlus, ex);
            }
        }

        /// <inheritdoc />
        public async Task ExecuteArg1IntAsync(string caller, string procedureName, int arg1, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteArg1IntAsync)}";

            try
            {
                ValidateProcedureName(callChain, procedureName);

                if (arg1 <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'argument 'arg1' fourni à la procédure stockée doit être strictement positif (reçu : {arg1}).");

                ct.ThrowIfCancellationRequested();

                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                string sql = $"EXEC {procedureName} @p0;";
                await context.Database.ExecuteSqlRawAsync(sql, new object[] { arg1 }, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                string callChainPlus = $"{callChain} > Stored procedure failed: {procedureName} (arg1={arg1}) > {DescribeSqlError(ex)}";

                throw _classifier.Execute(callChainPlus, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Valide le nom de procédure stockée par contrôle de non-vacuité puis appartenance à la whitelist.
        /// </summary>
        /// <remarks>
        /// <para>Contexte</para>
        /// <para>Le nom d'une procédure ne peut être paramétré et doit donc être contrôlé en amont (anti-injection par nom).</para>
        /// <para>Objectif</para>
        /// <para>Empêcher toute exécution arbitraire en levant <see cref="Ex_Business"/> typée à code <c>BU_ER_01</c> en cas de non-conformité.</para>
        /// <para>Responsabilités</para>
        /// <list type="bullet">
        /// <item><description>Lever <see cref="Ex_Business"/> code <c>BU_ER_01</c> si le nom est null/vide.</description></item>
        /// <item><description>Lever <see cref="Ex_Business"/> code <c>BU_ER_01</c> si le nom n'appartient pas à la whitelist.</description></item>
        /// </list>
        /// <para>Non-responsabilités</para>
        /// <list type="bullet">
        /// <item><description>Ne capte ni ne requalifie aucune exception — les <see cref="Ex_Business"/> levées remontent jusqu'au <c>catch (Ex_Business) { throw; }</c> de la méthode publique appelante.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">CallChain construite par la méthode publique appelante.</param>
        /// <param name="procedureName">Nom de procédure à valider.</param>
        /// <exception cref="Ex_Business">
        /// Code <c>BU_ER_01</c> si <paramref name="procedureName"/> est null/vide ou non whitelisté.
        /// </exception>
        private void ValidateProcedureName(string caller, string procedureName)
        {
            string callChain = $"{caller} > {nameof(ValidateProcedureName)}";

            if (string.IsNullOrWhiteSpace(procedureName))
                throw new Ex_Business(
                    callChain,
                    Ex_Business.ErrorCodes.BU_ER_01,
                    "Le nom de procédure stockée fourni est null ou vide.");

            if (!_allowedProcedures.Contains(procedureName))
                throw new Ex_Business(
                    callChain,
                    Ex_Business.ErrorCodes.BU_ER_01,
                    $"La procédure stockée '{procedureName}' n'est pas autorisée par la whitelist.");
        }

        /// <summary>
        /// Formate un diagnostic synthétique d'une exception SQL à des fins d'enrichissement de la
        /// CallChain remise au pipeline de requalification, sans journalisation ni notification.
        /// </summary>
        /// <remarks>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisée dans le quatrième catch terminal des méthodes publiques pour enrichir la chaîne
        /// transmise à <see cref="IS_ExClassifier.Execute"/>. L'enrichissement est conforme à §4.5
        /// et §4.7 du 0230 : il ne constitue ni une journalisation au sens prohibé par <c>I-4.7.6</c>,
        /// ni une notification.
        /// </para>
        /// <para>Objectif</para>
        /// <para>Fournir une caractérisation lisible des erreurs SQL Server fréquentes (clé dupliquée, contrainte, conversion, deadlock, timeout, etc.) pour faciliter le diagnostic en aval.</para>
        /// <para>Responsabilités</para>
        /// <list type="bullet">
        /// <item><description>Décapsuler une éventuelle <c>DbUpdateException</c> pour atteindre l'exception racine.</description></item>
        /// <item><description>Reconnaître les codes <c>SqlException.Number</c> usuels et produire une étiquette lisible.</description></item>
        /// </list>
        /// <para>Non-responsabilités</para>
        /// <list type="bullet">
        /// <item><description>N'émet aucune sortie vers un canal de log ou de notification.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="ex">Exception à caractériser.</param>
        /// <returns>Chaîne diagnostique synthétique destinée à enrichir la CallChain.</returns>
        private static string DescribeSqlError(Exception ex)
        {
            Exception root = ex;

            if (ex is DbUpdateException dbu && dbu.InnerException != null)
                root = dbu.InnerException;

            root = root.GetBaseException();

            if (root is SqlException sql)
            {
                string kind = sql.Number switch
                {
                    2627 or 2601 => "Duplicate key (unique constraint/index)",
                    547 => "Foreign key / check constraint violation",
                    515 => "Cannot insert NULL (NOT NULL violation)",
                    8115 => "Arithmetic overflow / numeric out of range",
                    245 => "Conversion failed",
                    1205 => "Deadlock victim",
                    -2 => "Timeout",
                    _ => "SQL error"
                };

                return $"{kind}. SqlException(Number={sql.Number}, State={sql.State}, Line={sql.LineNumber}, Proc={sql.Procedure}) : {sql.Message}";
            }

            return $"{root.GetType().Name} : {root.Message}";
        }

        #endregion
    }
}