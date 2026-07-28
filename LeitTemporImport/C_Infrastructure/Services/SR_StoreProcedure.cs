using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LeitTemporImport.C_Infrastructure.Services
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service d’exécution de procédures stockées SQL Server pour orchestrer des opérations techniques
    /// (import, synchronisation, consolidation) via EF Core, tout en respectant les standards projet 104 :
    /// CallChain, reclassification d’exceptions, traçabilité.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases d’import MDB → SQL Server 2019, lorsque certaines étapes sont implémentées
    /// sous forme de procédures stockées côté base.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Exécuter des procédures stockées de manière sûre (paramétrée), contrôlée (whitelist) et traçable.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases et Services applicatifs du projet 104.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Exécuter une procédure sans paramètre.</description></item>
    /// <item><description>Exécuter une procédure avec 1 paramètre.</description></item>
    /// <item><description>Garantir la traçabilité et la classification d’erreurs.</description></item>
    /// </list>
    /// </summary>
    public class SR_StoredProcedure : IS_StoredProcedure
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IDbContextFactory<DigitTryDbContext> _contextFactory;

        /// <summary>
        /// Liste blanche des procédures autorisées (anti-injection via nom de procédure).
        /// </summary>
        private readonly ISet<string> _allowedProcedures;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service d’exécution de procédures stockées.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche Infrastructure.</para>
        /// <para>Objectif</para>
        /// <para>Garantir la disponibilité du DbContextFactory et du contrôle de sécurité (whitelist).</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// <item><description>Valider les dépendances.</description></item>
        /// </list>
        /// <param name="contextFactory">Factory EF Core pour créer un DbContext par exécution.</param>
        /// <param name="allowedProcedures">Liste blanche de procédures autorisées.</param>
        /// <exception cref="ArgumentNullException">Si une dépendance est nulle.</exception>
        /// </summary>
        public SR_StoredProcedure(
            IDbContextFactory<DigitTryDbContext> contextFactory,
            ISet<string> allowedProcedures)
        {
            _callee = GetType().Name;
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _allowedProcedures = allowedProcedures ?? throw new ArgumentNullException(nameof(allowedProcedures));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute une procédure stockée sans paramètre.</para>
        /// <para>Contexte</para>
        /// <para>Appelée par un UseCase orchestrateur d’import.</para>
        /// <para>Objectif</para>
        /// <para>Déclencher un traitement SQL côté serveur en garantissant sécurité et traçabilité.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le nom de procédure via whitelist.</description></item>
        /// <item><description>Créer un DbContext et exécuter la procédure.</description></item>
        /// <item><description>Reclassifier toute exception via <c>Ex_Classifier</c>.</description></item>
        /// </list>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="procedureName">Nom de la procédure stockée.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <exception cref="Exception">Reclassifiée via Ex_Classifier.</exception>
        /// </summary>
        public async Task ExecuteAsync(string caller, string procedureName, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ValidateProcedureName(callChain, procedureName);

                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                string sql = $"EXEC {procedureName};";
                await context.Database.ExecuteSqlRawAsync(sql, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                string details = DescribeSqlError(ex);

                throw Ex_Classifier.Execute(
                    callChain,
                    new Ex_Infrastructure($"Stored procedure failed: {procedureName}. {details}", ex));
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute une procédure stockée avec 1 paramètre générique hor int/string.</para>
        /// <para>Contexte</para>
        /// <para>Utilisée lorsque la procédure attend un argument (ex : numéro de série).</para>
        /// <para>Objectif</para>
        /// <para>Passer un paramètre de façon sûre (paramétrée), sans concaténation SQL dangereuse.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le nom de procédure via whitelist.</description></item>
        /// <item><description>Exécuter <c>EXEC Proc @p0</c> via paramètres EF Core.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="procedureName">Nom de la procédure stockée.</param>
        /// <param name="arg1">Valeur du premier paramètre.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <exception cref="Exception">Reclassifiée via Ex_Classifier.</exception>
        public async Task ExecuteArg1Async(string caller, string procedureName, object arg1, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteArg1Async)}";

            try
            {
                ValidateProcedureName(callChain, procedureName);

                if (arg1 is null)
                    throw new ArgumentNullException(nameof(arg1), "Argument must be provided.");

                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                string sql = $"EXEC {procedureName} @p0;";
                await context.Database.ExecuteSqlRawAsync(sql, new object[] { arg1 }, ct);
            }
            catch (Exception ex)
            {
                string details = DescribeSqlError(ex);
                throw Ex_Classifier.Execute(
                    callChain,
                    new Ex_Infrastructure($"Stored procedure failed: {procedureName} (arg1={arg1}). {details}", ex));
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute une procédure stockée avec 1 paramètre de type <c>string</c>.</para>
        /// <para>Contexte</para>
        /// <para>Utilisée lorsque la procédure attend un argument alphanumérique de type string.</para>
        /// <para>Objectif</para>
        /// <para>Passer un paramètre de façon sûre (paramétrée), sans concaténation SQL dangereuse.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le nom de procédure via whitelist.</description></item>
        /// <item><description>Exécuter <c>EXEC Proc @p0</c> via paramètres EF Core.</description></item>
        /// </list>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="procedureName">Nom de la procédure stockée.</param>
        /// <param name="arg1">Valeur du premier paramètre.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <exception cref="Exception">Reclassifiée via Ex_Classifier.</exception>
        /// </summary>
        public async Task ExecuteArg1StringAsync(string caller, string procedureName, string arg1, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteArg1StringAsync)}";

            try
            {
                ValidateProcedureName(callChain, procedureName);

                if (string.IsNullOrWhiteSpace(arg1))
                    throw new ArgumentException("Argument must be provided.", nameof(arg1));

                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                string sql = $"EXEC {procedureName} @p0;";
                await context.Database.ExecuteSqlRawAsync(sql, new object[] { arg1 }, ct);
            }
            catch (Exception ex)
            {
                string details = DescribeSqlError(ex);
                throw Ex_Classifier.Execute(
                    callChain,
                    new Ex_Infrastructure($"Stored procedure failed: {procedureName} (arg1={arg1}). {details}", ex));
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute une procédure stockée avec 1 paramètre de type <c>int</c>.</para>
        /// <para>Contexte</para>
        /// <para>Utilisée pour les procédures attendues avec un identifiant numérique de type int (ex : IdSerialNumber).</para>
        /// <para>Objectif</para>
        /// <para>Fournir une exécution typée, sûre et explicite.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le nom de procédure via whitelist.</description></item>
        /// <item><description>Valider la valeur <paramref name="arg1"/>.</description></item>
        /// <item><description>Exécuter la procédure via paramètre EF Core.</description></item>
        /// </list>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="procedureName">Nom de la procédure stockée.</param>
        /// <param name="arg1">Valeur du premier paramètre.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <exception cref="Exception">Reclassifiée via Ex_Classifier.</exception>
        /// </summary>
        public async Task ExecuteArg1IntAsync(string caller, string procedureName, int arg1, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteArg1IntAsync)}";

            try
            {
                ValidateProcedureName(callChain, procedureName);

                if (arg1 <= 0)
                    throw new ArgumentOutOfRangeException(nameof(arg1), "Argument must be > 0.");

                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                string sql = $"EXEC {procedureName} @p0;";
                await context.Database.ExecuteSqlRawAsync(sql, new object[] { arg1 }, ct);
            }
            catch (Exception ex)
            {
                string details = DescribeSqlError(ex);
                throw Ex_Classifier.Execute(
                    callChain,
                    new Ex_Infrastructure($"Stored procedure failed: {procedureName} (arg1={arg1}). {details}", ex));
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Valide le nom de procédure stockée via une liste blanche.</para>
        /// <para>Contexte</para>
        /// <para>Le nom d’une procédure ne peut pas être paramétré, donc doit être contrôlé.</para>
        /// <para>Objectif</para>
        /// <para>Empêcher toute exécution arbitraire (injection via nom de procédure).</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Vérifier null/empty.</description></item>
        /// <item><description>Vérifier appartenance à la whitelist.</description></item>
        /// </list>
        /// <param name="caller">Chaîne de traçabilité.</param>
        /// <param name="procedureName">Nom de procédure.</param>
        /// <exception cref="ArgumentException">Si le nom est vide.</exception>
        /// <exception cref="InvalidOperationException">Si la procédure n’est pas autorisée.</exception>
        /// </summary>
        private void ValidateProcedureName(string caller, string procedureName)
        {
            string callChain = $"{caller} > {nameof(ValidateProcedureName)}";

            try
            {
                if (string.IsNullOrWhiteSpace(procedureName))
                    throw new ArgumentException("Procedure name is required.", nameof(procedureName));

                if (!_allowedProcedures.Contains(procedureName))
                    throw new InvalidOperationException($"Stored procedure '{procedureName}' is not allowed. Caller='{caller}'.");
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        private static string DescribeSqlError(Exception ex)
        {
            // Unwrap
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

