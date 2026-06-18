using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Commands;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;

namespace DG244Cutting.B_UseCases.Handlers.Commands
{
    /// <summary>
    /// Command Handler responsable de la persistance des enregistrements de l'Event Store applicatif.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans B_UseCases et implémente <see cref="IC_UserAppEventStore"/>.
    /// Elle est le seul composant de la solution autorisé à inscrire des enregistrements dans la
    /// table <c>UserAppEventStore</c>. Elle est appelée exclusivement par <c>CH_Generic&lt;T&gt;</c>
    /// via sa méthode privée de journalisation — jamais directement depuis un Service, un UseCase
    /// ou un ViewModel.
    /// </para>
    /// <para>
    /// Enrichissement contextuel : chaque enregistrement est automatiquement complété avec les
    /// données du contexte applicatif courant (identité utilisateur, poste, adresse IP, horodatage)
    /// via <see cref="IS_AppContext"/>. Cette collecte est transparente pour le composant appelant.
    /// </para>
    /// <para>
    /// Solidarité transactionnelle : l'inscription dans le change tracker via le repository s'effectue
    /// dans la même transaction que la mutation métier qu'elle accompagne. La persistance finale
    /// est déclenchée par le UseCase orchestrateur, jamais par ce handler.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Valider les paramètres structurels entrants avant toute construction d'entité.</description></item>
    ///   <item><description>Construire l'entité <see cref="UserAppEventStore"/> en agrégeant les paramètres reçus et le contexte applicatif.</description></item>
    ///   <item><description>Inscrire l'entité dans le change tracker via <see cref="IR_Generic{T}"/>.</description></item>
    ///   <item><description>Requalifier les exceptions non contrôlées via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne journalise pas, ne notifie pas : ces responsabilités appartiennent au UseCase via <c>IU_LogAndNotify</c>.</description></item>
    ///   <item><description>Ne persiste pas les changements : responsabilité exclusive du UseCase orchestrateur.</description></item>
    ///   <item><description>Ne contient aucune logique métier relative aux entités tracées.</description></item>
    /// </list>
    /// </remarks>
    public class CH_UserAppEventStore : IC_UserAppEventStore
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        /// <summary>Repository de persistance des enregistrements <see cref="UserAppEventStore"/>.</summary>
        private readonly IR_Generic<UserAppEventStore> _repository;

        /// <summary>
        /// Query Handler fournissant le contexte applicatif courant : identité utilisateur,
        /// poste de travail, adresse IP et horodatage applicatif.
        /// </summary>
        private readonly IS_AppContext _appContext;

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés
        /// (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="CH_UserAppEventStore"/> avec ses dépendances opérationnelles.
        /// </summary>
        /// <param name="repository">
        /// Repository de persistance des enregistrements de l'Event Store.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="appContext">
        /// Query Handler fournissant le contexte applicatif courant pour l'enrichissement de chaque
        /// enregistrement. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="repository"/>, <paramref name="appContext"/> ou
        /// <paramref name="classifier"/> est <see langword="null"/>.
        /// </exception>
        public CH_UserAppEventStore(
            IR_Generic<UserAppEventStore> repository,
            IS_AppContext appContext,
            IS_ExClassifier classifier)
        {
            _callee = GetType().Name;

            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion


        #region === Méthodes publiques ===

        /// <summary>
        /// Inscrit un enregistrement Event Store contextualisé dans le DbContext partagé.
        /// </summary>
        /// <remarks>
        /// L'enregistrement produit est automatiquement enrichi avec le contexte applicatif courant
        /// (identité utilisateur, poste, adresse IP, horodatage) via <c>IQ_AppContext</c> dans
        /// l'implémentation. L'écriture s'effectue dans la même transaction que la mutation métier
        /// qu'elle accompagne, garantissant la solidarité transactionnelle définie en section 3.8
        /// du référentiel normatif.
        /// </remarks>
        /// <param name="caller">
        /// CallChain complète construite jusqu'au Command Handler métier ayant déclenché la mutation.
        /// Cette valeur est stockée telle quelle dans le champ <c>AppCallChain</c> de l'entité,
        /// conformément à l'exemple du référentiel normatif (section 3.7).
        /// </param>
        /// <param name="tableDesignation">
        /// Nom de l'entité ou de la table métier concernée par la mutation tracée.
        /// Ne doit pas être <see langword="null"/> ni vide.
        /// </param>
        /// <param name="tableId">
        /// Identifiant de l'enregistrement métier modifié. La valeur <c>0</c> est acceptée
        /// lorsque l'identifiant n'est pas disponible (ex. : entité sans propriété <c>Id</c> entière).
        /// </param>
        /// <param name="data">
        /// Sérialisation JSON complète de l'état de l'entité au moment de la mutation.
        /// Ne doit pas être <see langword="null"/> ni vide.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="tableDesignation"/> ou <paramref name="data"/> est nul ou vide.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'inscription dans le change tracker.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task HandleAddAsync(
            string caller,
            string tableDesignation,
            int tableId,
            string data,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleAddAsync)}";

            try
            {
                if (string.IsNullOrWhiteSpace(tableDesignation))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "La désignation de la table fournie pour l'enregistrement Event Store est nulle ou vide.");

                if (string.IsNullOrWhiteSpace(data))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Les données sérialisées fournies pour l'enregistrement Event Store sont nulles ou vides.");

                ct.ThrowIfCancellationRequested();

                DTO_AppContext appCtx = _appContext.GetAppContext();

                var entity = new UserAppEventStore
                {
                    TableDesignation = tableDesignation,
                    TableId = tableId,
                    Timestamp = appCtx.AppDateTime,
                    Data = data,
                    AppId = appCtx.AppId,
                    AppCallChain = caller,         // callChain jusqu'au handler métier appelant — cf. IC_UserAppEventStore
                    AppUserId = appCtx.AppUserId,
                    DeviceUser = appCtx.AppDeviceUser,
                    DeviceId = appCtx.AppDeviceId,
                    DeviceIp = appCtx.AppDeviceIP
                };

                await _repository.AddAsync(callChain, entity, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion


        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}