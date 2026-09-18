using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;

namespace BatchStockRelease.B_UseCases.Services.UserLogic
{
    /// <summary>
    /// Service de gestion des sessions utilisateurs.
    /// <para>
    /// Ce service orchestre la création, la mise à jour et la fermeture
    /// des sessions liées à un utilisateur et à une application spécifique.
    /// </para>
    /// </summary>
    public class SR_UserSession_Open : IS_UserSession_Open
    {
        #region === Propriétés privées ===
        /// <summary>
        /// Nom interne du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;
        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserSession _qhUserSession;
        private readonly IC_UserSession _chUserSession;
        private readonly IS_Settings_User _settingsUser;

        #endregion

        #region === Constructeur ===

        public SR_UserSession_Open(
            IC_UserSession userSessionCommand,
            IQ_UserSession userSessionQuery,
            IS_Settings_User settingsUser)
        {
            _callee = GetType().Name;

            _chUserSession = userSessionCommand;
            _qhUserSession = userSessionQuery;
            _settingsUser = settingsUser;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Ouvre une session utilisateur pour l’application courante.
        /// <para>
        /// Cette méthode vérifie les sessions existantes, les met à jour si nécessaire,
        /// en crée une nouvelle si aucune n’existe, et met à jour le contexte utilisateur.
        /// </para>
        /// </summary>
        /// <param name="userId">Identifiant utilisateur.</param>
        /// <param name="appId">Identifiant de l’application.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        public async Task ExecuteAsync(int userId, int appId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1️ : Vérifier et préparer l’état de session utilisateur
                var updated = await EnsureUserSessionStateAsync(userId, appId, callChain);

                // Étape 2 : Appliquer la mise à jour du contexte utilisateur
                if (updated)
                    await ApplyUserSessionUpdatesAsync(userId, appId);
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Vérifie les sessions existantes pour un utilisateur donné et prépare l’état de session.
        /// <para>
        /// Si des sessions existent :
        /// - Met à jour la première comme active.
        /// - Supprime les sessions supplémentaires.
        /// </para>
        /// Si aucune session n’existe, une nouvelle est créée.
        /// </summary>
        /// <param name="userId">Identifiant utilisateur.</param>
        /// <param name="appId">Identifiant application.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité.</param>
        /// <returns><c>true</c> si une mise à jour ou création a eu lieu.</returns>
        private async Task<bool> EnsureUserSessionStateAsync(int userId, int appId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {nameof(EnsureUserSessionStateAsync)}";

            var existingSessions = await _qhUserSession.HandleGetByUserIdAppIdAsync(userId, appId);

            if (existingSessions.Any())
            {
                // Mettre à jour la session principale comme active
                await _chUserSession.HandleUpdateUserSessionAsync(existingSessions.First(), true, callChain);

                // Supprimer les sessions redondantes
                if (existingSessions.Skip(1).Any())
                    await _chUserSession.HandleDeleteAdditionalSessions(existingSessions.Skip(1), callChain);

                return true;
            }
            else
            {
                // Créer une nouvelle session utilisateur
                await _chUserSession.HandleCreateNewUserSessionAsync(callChain);
                return true;
            }
        }

        /// <summary>
        /// Met à jour les informations de session dans les paramètres utilisateur.
        /// <para>
        /// Cette méthode recharge la session active depuis la base
        /// et met à jour la propriété <c>SessionId</c> de l’utilisateur courant.
        /// </para>
        /// </summary>
        private async Task ApplyUserSessionUpdatesAsync(int userId, int appId)
        {
            var existingSessions = await _qhUserSession.HandleGetByUserIdAppIdAsync(userId, appId);

            if (existingSessions.Any())
            {
                var entity = existingSessions.First();
                _settingsUser.SetSessionId(entity.Id);
            }
        }

        #endregion
    }
}