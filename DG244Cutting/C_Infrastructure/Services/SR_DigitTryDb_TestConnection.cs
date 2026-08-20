using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Infrastructure;
using DG244Cutting.C_Infrastructure.Persistence.DIGIT_TRY.Context;
using Microsoft.EntityFrameworkCore;

namespace DG244Cutting.C_Infrastructure.Services
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service de diagnostic de connectivité à la base de données SQL Server portée par
    /// <see cref="DigitTryDbContext"/>. Implémente <see cref="IS_DigitTryDb_TestConnection"/>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Réside en <c>C_Infrastructure/Services/</c> conformément à la deuxième obligation
    /// contractuelle de §4.14.3 amendée (interface en <c>Services/Infrastructure/</c> →
    /// implémentation en <c>C_Infrastructure/Services/</c>). Consomme la factory
    /// <see cref="IDbContextFactory{TContext}"/> de <see cref="DigitTryDbContext"/>
    /// enregistrée au Pattern 3 du câblage triple (§4.8.5 du 0230, R-4.8.9 du 0231),
    /// au titre d'un troisième cas d'usage de cette factory hors transaction UseCase,
    /// porté par décision architecturale documentée préalable au sens de R-4.14.20 et
    /// homogène aux deux cas inventoriés (EA-09 / <c>CR_UserAppErrorLog</c> et
    /// <c>SR_StoredProcedure</c>).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Produire, à la demande d'un UseCase orchestrateur, un diagnostic binaire de
    /// connectivité à la base via la primitive EF Core
    /// <see cref="DatabaseFacade.CanConnectAsync(System.Threading.CancellationToken)"/>,
    /// sur un DbContext de courte durée disposé à l'issue de l'opération.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases orchestrateurs de démarrage et de re-test périodique de connectivité.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Produire un DbContext de courte durée via la factory EF Core injectée.</description></item>
    /// <item><description>Invoquer la primitive EF Core <c>Database.CanConnectAsync</c>.</description></item>
    /// <item><description>Propager le résultat binaire ; requalifier toute exception non prévue via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire la <c>CallChain</c> selon le format normatif §4.5 et la propager au classificateur en cas d'exception non prévue.</description></item>
    /// <item><description>Encadrer l'opération principale par le patron à quatre catch canonique (§4.7 ; R-4.7.1, R-4.7.6, R-4.7.25, R-4.6.13).</description></item>
    /// <item><description>Disposer le DbContext de courte durée en fin d'exécution via <c>await using</c>, qu'elle réussisse ou échoue.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide, ni n'annule aucune transaction (§3.8, §4.10 ; I-4.10.1).</description></item>
    /// <item><description>N'invoque aucun <c>SaveChangesAsync</c>, aucun Repository, aucun Command/Query Handler (I-4.14.6, I-4.14.9).</description></item>
    /// <item><description>Ne notifie pas <c>IS_AppContext</c> de l'état de connexion : cette responsabilité relève du UseCase orchestrateur.</description></item>
    /// <item><description>Ne journalise pas via <c>IS_ErrorLogger</c> et ne notifie pas via <c>IS_Notification</c> (I-4.7.6 ; hors-portée EA-09).</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_DigitTryDb_TestConnection"/>
    /// <seealso cref="IDbContextFactory{TContext}"/>
    /// <seealso cref="IS_ExClassifier"/>
    public class SR_DigitTryDb_TestConnection : IS_DigitTryDb_TestConnection
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom du composant courant, résolu dynamiquement par <c>GetType().Name</c> pour la
        /// construction du segment local de la CallChain (§4.5 ; R-4.5.5).
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Factory EF Core produisant des DbContexts de courte durée, indépendants de tout
        /// scope UseCase, enregistrée au Pattern 3 du câblage triple (§4.8.5 ; R-4.8.9).
        /// Chaque invocation de <see cref="ExecuteAsync"/> crée et dispose son propre contexte.
        /// </summary>
        private readonly IDbContextFactory<DigitTryDbContext> _contextFactory;

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs
        /// normalisés (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>),
        /// consommé dans le catch <c>Exception</c> terminal du patron à quatre catch
        /// (§4.7 ; R-4.7.25).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service de diagnostic de connectivité.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche Infrastructure (Singleton, cf. enregistrement
        /// dans <c>SR_ConteneurDI.RDI_Services</c>, sous-section <c>// Infrastructure</c>).</para>
        /// <para>Objectif</para>
        /// <para>Garantir la disponibilité de la factory EF Core et du classificateur d'exceptions.</para>
        /// </summary>
        /// <param name="contextFactory">
        /// Factory EF Core produisant les DbContexts de courte durée. Ne doit pas être
        /// <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées. Ne doit pas être
        /// <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si l'un des paramètres est <see langword="null"/>.
        /// </exception>
        public SR_DigitTryDb_TestConnection(
            IDbContextFactory<DigitTryDbContext> contextFactory,
            IS_ExClassifier classifier)
        {
            _callee = GetType().Name;
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <inheritdoc/>
        public async Task<bool> ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Aucune précondition structurelle à valider : la méthode publique ne reçoit
                // que la CallChain amont et le jeton d'annulation. Position validation -> ct
                // dégénérée à ct seul, conforme à §4.7 (R-4.7.25).
                ct.ThrowIfCancellationRequested();

                // DbContext de courte durée produit par la factory du Pattern 3 (§4.8.5).
                // L'instruction await using garantit la disposition du contexte à l'issue
                // de l'opération, qu'elle réussisse ou échoue.
                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                return await context.Database.CanConnectAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}