using BatchStockRelease.A_Domain.Common.Enums;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic;
using System.ComponentModel;
using System.Windows;

namespace BatchStockRelease.B_UseCases.UseCases.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// UseCase orchestrateur responsable de la fermeture complète de l’application.
    /// Il gère la confirmation utilisateur, la déconnexion propre de la session,
    /// et retourne un résultat permettant à la couche UI de décider de la suite du flux.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce UseCase est exécuté lors de l’événement <c>OnClosing</c> de la fenêtre principale.
    /// </para>
    /// </summary>
    public class UC_User_CloseApplication : IU_User_CloseApplication
    {
        #region === Propriétés privées ===
        /// <summary>
        /// Nom du UseCase pour la traçabilité.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances injectées ===

        private readonly IS_Settings_User _settingsUser;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_UserSession_Close _userSessionClose;
        private readonly IS_Notification _notification;
        private readonly IS_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        public UC_User_CloseApplication(
            IS_Settings_User settingsUser,
            IS_Settings_App settingsApp,
            IS_UserSession_Close userSessionClose,
            IS_Notification notification,
            IS_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;

            _settingsUser = settingsUser;
            _settingsApp = settingsApp;
            _userSessionClose = userSessionClose;
            _notification = notification;
            _logAndNotify = logAndNotify;
        }

        #endregion

        #region === Méthode publique ===

        /// <summary>
        /// Exécute la logique de fermeture de l’application et retourne le résultat à la couche UI.
        /// </summary>
        /// <param name="e">Arguments de l’événement de fermeture.</param>
        /// <param name="caller">Nom du composant appelant (pour la traçabilité).</param>
        /// <returns>Un <see cref="En_CloseResult"/> indiquant le statut de fermeture.</returns>
        public async Task<En_CloseResult> ExecuteAsync(CancelEventArgs e, string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            int userId = _settingsUser.GetAppUserId();
            int sessionId = _settingsUser.GetSessionId();

            try
            {
                // Étape 1 : Fermeture forcée
                if (_settingsUser.GetForceClose())
                {
                    // Vérifier si il y a une connexion à la base de données
                    if (_settingsApp.GetIsConnected())
                    {
                        await _userSessionClose.ExecuteAsync(userId, sessionId, callChain);
                    }
                    else
                    {
                        await _logAndNotify.ExecuteAsync("No_EC_19", new Ex_Infrastructure(callChain, "UCA_01", "No_Wa_18"), false);
                    }

                    return En_CloseResult.ForceClosed;
                }

                // Étape 2 : Demande de confirmation
                var result = _notification.ConfirmationReturn("No_AD_01");

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return En_CloseResult.Cancelled;
                }

                // Étape 3 : Déconnexion contrôlée
                e.Cancel = true;
                await _userSessionClose.ExecuteAsync(userId, sessionId, callChain);

                return En_CloseResult.Closed;
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", ex);
                return En_CloseResult.Cancelled;
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", ex);
                return En_CloseResult.Cancelled;
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
                return En_CloseResult.Cancelled;
            }
        }

        #endregion
    }
}