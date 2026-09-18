using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.DTOs.App;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Commands;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using LeitTemporImport.B_UseCases.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Handlers.Commands
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// CommandHandler dédié à <see cref="UserAppSession"/> pour gérer les écritures liées aux sessions
    /// (ouverture/fermeture du programme). S’appuie sur <see cref="CH_Generic{T}"/> pour exécuter les commandes
    /// et déclencher la journalisation snapshot dans l’Event Store.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé au démarrage et à l’arrêt de l’application console afin de créer et mettre à jour l’état
    /// des sessions utilisateur.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser la création et la mise à jour des sessions en renseignant les informations d’application,
    /// utilisateur et device à partir du <see cref="IQ_AppContext"/>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Créer une nouvelle session connectée.</description></item>
    /// <item><description>Mettre à jour une session (connecté/déconnecté) avec dates et device.</description></item>
    /// <item><description>Supprimer des sessions supplémentaires si nécessaire.</description></item>
    /// </list>
    /// </summary>
    public class CH_UserSession : CH_Generic<UserAppSession>, IC_UserSession
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_AppContext _appContext;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le CommandHandler <see cref="CH_UserSession"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Initialiser les dépendances nécessaires à l’écriture et à la journalisation (via la classe de base),
        /// ainsi que l’accès au contexte applicatif.
        /// </para>
        /// </summary>
        /// <param name="repository">Repository générique de <see cref="UserAppSession"/>.</param>
        /// <param name="eventStore">CommandHandler Event Store.</param>
        /// <param name="appContext">QueryHandler fournissant le contexte applicatif.</param>
        /// <exception cref="ArgumentNullException">Si une dépendance est null.</exception>
        public CH_UserSession(
            IR_UserSession repository,
            IQ_AppContext appContext)
            : base(repository ?? throw new ArgumentNullException(nameof(repository)))
        {
            _callee = GetType().Name;
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Crée un nouvel enregistrement <see cref="UserAppSession"/> en état connecté.</para>
        /// <para>Contexte</para>
        /// <para>Appelé à l’ouverture du programme.</para>
        /// <para>Objectif</para>
        /// <para>Insérer une session avec les informations d’application/utilisateur/device courantes.</para>
        /// </summary>
        /// <param name="callChain">Chaîne d’appel amont (non modifiée).</param>
        /// <exception cref="Exception">Toute exception est reclassifiée via <see cref="Ex_Classifier"/>.</exception>
        public async Task HandleCreateNewUserSessionAsync(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleCreateNewUserSessionAsync)}";

            try
            {
                DTO_AppContext appCtx = _appContext.GetAppContext();

                var entity = new UserAppSession
                {
                    IdApplication = appCtx.AppId,
                    IdUser = appCtx.AppUserId,
                    DeviceUser = appCtx.AppDeviceUser,
                    DeviceId = appCtx.AppDeviceId,
                    DeviceIp = appCtx.AppDeviceIP,
                    IsConnected = true,
                    ConnectionDate = appCtx.AppDateTime,

                    // Important : pas de date de déconnexion à la création d’une session connectée
                    DisconnectionDate = null
                };

                await HandleAddAsync(callChain, entity);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour une session avec l’état connecté/déconnecté et la date correspondante.</para>
        /// <para>Contexte</para>
        /// <para>Appelé lors de l’ouverture (connecté) ou fermeture (déconnecté) du programme.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Mettre à jour les informations device, le statut <c>IsConnected</c> et la date de connexion ou déconnexion.
        /// </para>
        /// </summary>
        /// <param name="entity">Session à mettre à jour.</param>
        /// <param name="isConnected">Nouvel état connecté/déconnecté.</param>
        /// <param name="callChain">Chaîne d’appel amont (non modifiée).</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// <exception cref="Exception">Toute exception est reclassifiée via <see cref="Ex_Classifier"/>.</exception>
        public async Task HandleUpdateUserSessionAsync(UserAppSession entity, bool isConnected, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleUpdateUserSessionAsync)}";

            try
            {
                if (entity is null) throw new ArgumentNullException(nameof(entity));

                DTO_AppContext appCtx = _appContext.GetAppContext();

                entity.DeviceUser = appCtx.AppDeviceUser;
                entity.DeviceId = appCtx.AppDeviceId;
                entity.DeviceIp = appCtx.AppDeviceIP;

                entity.IsConnected = isConnected;

                if (isConnected)
                {
                    entity.ConnectionDate = appCtx.AppDateTime;
                    entity.DisconnectionDate = null;
                }
                else
                {
                    entity.DisconnectionDate = appCtx.AppDateTime;
                }

                await HandleUpdateAsync(callChain, entity);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Supprime des sessions supplémentaires (hard delete) afin d’éviter les doublons.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé si une règle impose de ne conserver qu’une session pertinente.</para>
        /// <para>Objectif</para>
        /// <para>Supprimer physiquement les sessions fournies et journaliser la suppression.</para>
        /// </summary>
        /// <param name="additionalSessions">Sessions à supprimer.</param>
        /// <param name="callChain">Chaîne d’appel amont (non modifiée).</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="additionalSessions"/> est null.</exception>
        /// <exception cref="Exception">Toute exception est reclassifiée via <see cref="Ex_Classifier"/>.</exception>
        public async Task HandleDeleteAdditionalSessionsAsync(IEnumerable<UserAppSession> additionalSessions, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleCreateNewUserSessionAsync)}";

            try
            {
                if (additionalSessions is null) throw new ArgumentNullException(nameof(additionalSessions));

                foreach (var entity in additionalSessions)
                {
                    if (entity is null) continue;
                    if (entity.Id <= 0) continue;

                    await HandleHardDeleteAsync(callChain, entity.Id);
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}
