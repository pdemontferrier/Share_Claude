using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Commands;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Services.User;
using LeitTemporImport.A_Domain.Interfaces.Settings.User;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Services.User
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service User d’ouverture de session applicative (<see cref="UserAppSession"/>).
    /// Orchestration : détecter les sessions existantes pour un utilisateur et une application,
    /// activer la session pertinente (connectée), supprimer les sessions redondantes,
    /// ou créer une nouvelle session si nécessaire, puis enregistrer l’identifiant de session
    /// dans les paramètres utilisateur (<see cref="ISE_User"/>).
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Appelé au démarrage du programme console, après identification de l’utilisateur et du device.
    /// La session est utilisée ensuite pour assurer une traçabilité cohérente entre les écritures
    /// applicatives, l’Event Store et les logs d’erreur.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Garantir qu’une session “active” existe pour l’utilisateur courant sur l’application courante,
    /// et que son identifiant soit disponible dans le contexte utilisateur global.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases User (ex : UC_UserIdentify) et pipeline d’initialisation console.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Rechercher les sessions existantes (userId + appId).</description></item>
    /// <item><description>Sélectionner la session principale et la marquer connectée.</description></item>
    /// <item><description>Supprimer les sessions supplémentaires.</description></item>
    /// <item><description>Créer une session si aucune n’existe.</description></item>
    /// <item><description>Mettre à jour <c>AppSessionId</c> via <see cref="ISE_User"/>.</description></item>
    /// <item><description>Reclassifier les exceptions via <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_UserSession_Open : IS_UserSession_Open
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserSession _qhUserSession;
        private readonly IC_UserSession _chUserSession;
        private readonly ISE_User _settingsUser;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service <see cref="SR_UserSession_Open"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche UseCases.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser les dépendances nécessaires à l’orchestration d’ouverture de session.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider les dépendances injectées.</description></item>
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="userSessionCommand">CommandHandler de <see cref="UserAppSession"/>.</param>
        /// <param name="userSessionQuery">QueryHandler de <see cref="UserAppSession"/>.</param>
        /// <param name="settingsUser">Service de paramètres utilisateur.</param>
        /// <exception cref="ArgumentNullException">Si une dépendance est null.</exception>
        public SR_UserSession_Open(
            IC_UserSession userSessionCommand,
            IQ_UserSession userSessionQuery,
            ISE_User settingsUser)
        {
            _callee = GetType().Name;

            _chUserSession = userSessionCommand ?? throw new ArgumentNullException(nameof(userSessionCommand));
            _qhUserSession = userSessionQuery ?? throw new ArgumentNullException(nameof(userSessionQuery));
            _settingsUser = settingsUser ?? throw new ArgumentNullException(nameof(settingsUser));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Ouvre (ou réactive) une session utilisateur pour l’application courante.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Appelé au démarrage du programme console. Le couple (userId, appId) identifie le périmètre
        /// de recherche des sessions existantes.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Assurer qu’une session active existe, supprimer les doublons éventuels,
        /// puis enregistrer l’identifiant de session dans les paramètres utilisateur.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Construire la callChain.</description></item>
        /// <item><description>Préparer l’état de session (update / delete / create).</description></item>
        /// <item><description>Mettre à jour <c>AppSessionId</c> dans <see cref="ISE_User"/>.</description></item>
        /// </list>
        /// <param name="userId">Identifiant utilisateur.</param>
        /// <param name="appId">Identifiant application.</param>
        /// <param name="caller">CallChain amont.</param>
        /// <returns>Tâche asynchrone.</returns>
        /// </summary>
        public async Task ExecuteAsync(string caller, int userId, int appId)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId), "userId must be > 0.");
                if (appId <= 0) throw new ArgumentOutOfRangeException(nameof(appId), "appId must be > 0.");
                if (string.IsNullOrWhiteSpace(caller)) throw new ArgumentException("caller cannot be empty.", nameof(caller));

                await EnsureUserSessionStateAsync(userId, appId, callChain);
                await ApplyUserSessionUpdatesAsync(userId, appId, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        private async Task EnsureUserSessionStateAsync(int userId, int appId, string caller)
        {
            string callChain = $"{caller} > {nameof(EnsureUserSessionStateAsync)}";

            try
            {
                var existingSessions = await _qhUserSession.HandleGetByUserIdAppIdAsync(callChain, userId, appId);

                if (existingSessions.Count == 0)
                {
                    await _chUserSession.HandleCreateNewUserSessionAsync(callChain);
                    return;
                }

                // Sélection déterministe de la session principale :
                // 1) session connectée prioritaire
                // 2) UpdatedAt la plus récente, puis CreatedAt, puis Id
                UserAppSession main = existingSessions
                    .OrderByDescending(s => s.IsConnected)
                    .ThenByDescending(s => s.UpdatedAt ?? DateTime.MinValue)
                    .ThenByDescending(s => s.CreatedAt)
                    .ThenByDescending(s => s.Id)
                    .First();

                // Activer/mettre à jour la session principale
                await _chUserSession.HandleUpdateUserSessionAsync(main, true, callChain);

                // Supprimer les autres sessions
                var additional = existingSessions.Where(s => s.Id != main.Id).ToList();
                if (additional.Count > 0)
                    await _chUserSession.HandleDeleteAdditionalSessionsAsync(additional, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        private async Task ApplyUserSessionUpdatesAsync(int userId, int appId, string caller)
        {
            string callChain = $"{caller} > {nameof(ApplyUserSessionUpdatesAsync)}";

            try
            {
                var existingSessions = await _qhUserSession.HandleGetByUserIdAppIdAsync(callChain, userId, appId);

                if (existingSessions.Count == 0)
                    throw new Ex_Business($"No UserAppSession found after EnsureUserSessionState for userId={userId}, appId={appId}.");

                var selected = existingSessions
                    .OrderByDescending(s => s.IsConnected)
                    .ThenByDescending(s => s.UpdatedAt ?? DateTime.MinValue)
                    .ThenByDescending(s => s.CreatedAt)
                    .ThenByDescending(s => s.Id)
                    .First();

                _settingsUser.SetAppSessionId(selected.Id);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion
    }
}