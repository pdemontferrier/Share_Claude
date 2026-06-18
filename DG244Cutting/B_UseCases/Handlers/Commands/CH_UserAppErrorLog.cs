using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Commands;
using DG244Cutting.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Services.App;

namespace DG244Cutting.B_UseCases.Handlers.Commands
{
    /// <summary>
    /// Command Handler responsable de la persistance immédiate et autonome des enregistrements
    /// de log d'erreurs applicatifs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans B_UseCases/Handlers/Commands et implémente
    /// <see cref="IC_UserAppErrorLog"/>. Elle constitue le maillon intermédiaire entre
    /// <c>SR_ErrorLogger</c> (qui construit l'entité) et <c>CR_UserAppErrorLog</c> (qui
    /// la persiste dans un DbContext de courte durée). Elle est appelée exclusivement par
    /// <c>SR_ErrorLogger</c> via le contrat <see cref="IC_UserAppErrorLog"/>.
    /// </para>
    /// <para>
    /// Objectif : valider l'entité reçue et déléguer immédiatement sa persistance à
    /// <see cref="IR_UserAppErrorLog"/>, en propageant la CallChain et en requalifiant
    /// les exceptions imprévues via <see cref="IS_ExClassifier"/>.
    /// </para>
    /// <para>
    /// Exception architecturale de persistance : contrairement aux Command Handlers génériques
    /// (<c>CH_Generic</c>), ce handler ne s'inscrit pas dans la transaction du UseCase
    /// orchestrateur, ne déclenche pas d'enregistrement dans l'Event Store et ne partage pas
    /// le DbContext du scope DI courant. Il délègue directement à <see cref="IR_UserAppErrorLog"/>,
    /// dont l'implémentation garantit une persistance autonome dans un DbContext de courte
    /// durée. Un log d'erreur doit survivre à tout rollback de la transaction principale.
    /// </para>
    /// <para>
    /// Rôle limité : ce handler reçoit l'entité entièrement construite par
    /// <c>SR_ErrorLogger</c> à partir du contexte applicatif et de l'exception normalisée.
    /// Il se limite à valider la présence de l'entité et à déléguer — il n'assemble pas
    /// les données lui-même.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Valider l'entité reçue avant toute délégation au repository.
    ///   </description></item>
    ///   <item><description>
    ///     Déléguer la persistance immédiate à <see cref="IR_UserAppErrorLog"/>.
    ///   </description></item>
    ///   <item><description>
    ///     Propager la CallChain et requalifier les exceptions imprévues via
    ///     <see cref="IS_ExClassifier"/>.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Ne construit pas l'entité : responsabilité exclusive de <c>SR_ErrorLogger</c>.
    ///   </description></item>
    ///   <item><description>
    ///     Ne journalise pas dans l'Event Store : les logs d'erreurs ne sont pas des
    ///     événements métier.
    ///   </description></item>
    ///   <item><description>
    ///     Ne journalise pas et ne notifie pas : aucune de ces responsabilités ne lui
    ///     appartient.
    ///   </description></item>
    /// </list>
    /// </remarks>
    public class CH_UserAppErrorLog : IC_UserAppErrorLog
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        /// <summary>
        /// Repository spécialisé assurant la persistance immédiate et autonome des enregistrements
        /// de log d'erreurs, indépendamment de toute transaction UseCase en cours.
        /// </summary>
        private readonly IR_UserAppErrorLog _repository;

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés
        /// (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="CH_UserAppErrorLog"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instanciée via le conteneur DI dans B_UseCases, avec enregistrement
        /// en tant que service transitoire ou singleton selon la politique de l'application.
        /// </para>
        /// <para>
        /// Objectif : préparer le handler avec le repository de persistance autonome et le
        /// classificateur d'exceptions nécessaires à son exécution robuste.
        /// </para>
        /// </remarks>
        /// <param name="repository">
        /// Repository spécialisé garantissant la persistance immédiate et indépendante
        /// des enregistrements <see cref="UserAppErrorLog"/>. Ne doit pas être
        /// <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées en types applicatifs
        /// normalisés. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="repository"/> ou <paramref name="classifier"/> est
        /// <see langword="null"/>.
        /// </exception>
        public CH_UserAppErrorLog(IR_UserAppErrorLog repository, IS_ExClassifier classifier)
        {
            _callee = GetType().Name;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion


        #region === Méthodes publiques ===

        /// <inheritdoc/>
        public async Task HandleAddAndSaveAsync(
            string caller,
            UserAppErrorLog entity,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleAddAndSaveAsync)}";

            try
            {
                if (entity is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "L'entité UserAppErrorLog fournie est nulle.");

                ct.ThrowIfCancellationRequested();

                await _repository.AddAndSaveAsync(callChain, entity, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion
    }
}