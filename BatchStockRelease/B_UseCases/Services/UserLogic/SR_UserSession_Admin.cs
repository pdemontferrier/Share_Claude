using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;

namespace BatchStockRelease.B_UseCases.Services.UserLogic
{
    public class SR_UserSession_Admin : IS_UserSession_Admin
    {
        private readonly string _callee;
        private readonly IC_UserSessionCommand _chUserSessionCommand;
        private readonly IQ_UserSessionCommand _qhUserSessionCommand;
        private readonly IQ_VieApplication _qhVieApplication;
        private readonly IS_Shutdown _application;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_User _settingsUser;
        private readonly IS_Notification _notification;

        public SR_UserSession_Admin(
            IC_UserSessionCommand uscCommand, 
            IQ_UserSessionCommand uscQuery, 
            IQ_VieApplication vaQuery,
            IS_Shutdown application, 
            IS_Settings_App settingsApp,
            IS_Settings_User settingsUser, 
            IS_Notification notification)
        {
            _callee = GetType().Name;
            _chUserSessionCommand = uscCommand;
            _qhUserSessionCommand = uscQuery;
            _qhVieApplication = vaQuery;
            _application = application;
            _settingsApp = settingsApp;
            _settingsUser = settingsUser;
            _notification = notification;
        }

        public async Task ListenForCommandsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Vérifie les commandes avec un interval de temps
                    await Task.Delay((_settingsApp.GetCloseCommandDelay() * 1000), cancellationToken);
                    await CheckForCloseCommandsAsync(cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Tâche annulée proprement
                    break;
                }
                catch (Exception ex)
                {
                    // Gérer les erreurs éventuelles
                    _notification.Error("No_EC_10", ex.Message);
                }
            }
        }


        private async Task CheckForCloseCommandsAsync(CancellationToken cancellationToken)
        {
            if (await IsCloseCommandForSessionAsync())
            {
                await NotifyAndShutdownAsync(cancellationToken);
            }
        }


        public async Task IssueCloseSessionCommandAsync(int targetSessionId)
        {
            string callChain = $"{_callee} > {nameof(IssueCloseSessionCommandAsync)}";

            await _chUserSessionCommand.HandleAddCloseSessionCommandAsync(targetSessionId, callChain);
        }


        private async Task NotifyAndShutdownAsync(CancellationToken cancellationToken)
        {
            string callChain = $"{_callee} > {nameof(NotifyAndShutdownAsync)}";

            // Afficher une fenêtre d'attente
            _notification.OpenDialogWindow("No_Ti_05", "No_Wa_08");

            // Attendre 5 secondes
            await Task.Delay(5000, cancellationToken);

            // Fermer DialogWindow
            _notification.CloseDialogWindow();

            // Fermer l'application
            await ForceClose(callChain);
        }


        private async Task<bool> IsCloseCommandForSessionAsync()
        {
            return await _qhUserSessionCommand.HandleExistsByCommandTypeUserIdAppIdAsync(_settingsUser.GetCloseCommandType(), _settingsUser.GetAppUserId(), _settingsApp.GetAppId());
        }


        private async Task ForceClose(string caller)
        {
            string callChain = $"{caller} > {nameof(ForceClose)}";

            await _chUserSessionCommand.HandleDeleteCloseCommandForSessionAsync(callChain);
            _application.ForceShutdown(callChain);
        }
    }
}