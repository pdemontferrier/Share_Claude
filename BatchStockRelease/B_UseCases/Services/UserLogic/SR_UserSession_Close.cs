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
    public class SR_UserSession_Close : IS_UserSession_Close
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

        #endregion

        #region === Constructeur ===

        public SR_UserSession_Close(
            IC_UserSession userSessionCommand,
            IQ_UserSession userSessionQuery)
        {
            _callee = GetType().Name;

            _chUserSession = userSessionCommand;
            _qhUserSession = userSessionQuery;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Ferme la session utilisateur spécifiée.
        /// </summary>
        /// <param name="userId">Identifiant utilisateur.</param>
        /// <param name="sessionId">Identifiant unique de la session à fermer.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        public async Task ExecuteAsync(int userId, int sessionId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (userId <= 0 || sessionId <= 0)
                    return;

                var existingSession = await _qhUserSession.HandleGetByIdAsync(sessionId);
                if (existingSession != null)
                {
                    await _chUserSession.HandleUpdateUserSessionAsync(existingSession, false, callChain);
                }
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}