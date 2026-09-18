using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;

namespace BatchCutting_DG.B_UseCases.Services.UserLogic
{
    public class SR_UserSessionsAdmin : IS_UserSessionsAdmin
    {
        private readonly string ServiceName;
        private readonly IC_UserSessionCommand _chUserSessionCommand;
        private readonly IQ_UserSessionCommand _qhUserSessionCommand;
        private readonly IQ_VieApplication _qhVieApplication;
        private readonly IS_Application _application;
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;

        public SR_UserSessionsAdmin(IC_UserSessionCommand uscCommand, IQ_UserSessionCommand uscQuery, IQ_VieApplication vaQuery,
                                        IS_Application application, IS_Settings settings, IS_Notification notification)
        {
            ServiceName = GetType().Name;
            _chUserSessionCommand = uscCommand;
            _qhUserSessionCommand = uscQuery;
            _qhVieApplication = vaQuery;
            _application = application;
            _settings = settings;
            _notification = notification;
        }


        public async Task<bool> CheckAppAccessibleAsync()
        {
            return await _qhVieApplication.HandleGetAppAccessibilityAsync(_settings.GetAppID());
        }


        public async Task ListenForCommandsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Vérifie les commandes avec un interval de temps
                    await Task.Delay((_settings.GetCloseCommandDelay() * 1000), cancellationToken);
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
            await _chUserSessionCommand.HandleAddCloseSessionCommandAsync(targetSessionId, ServiceName, nameof(IssueCloseSessionCommandAsync));
        }


        private async Task NotifyAndShutdownAsync(CancellationToken cancellationToken)
        {
            // Afficher une fenêtre d'attente
            _notification.OpenDialogWindow("No_Ti_05", "No_Wa_08");

            // Attendre 5 secondes
            await Task.Delay(5000, cancellationToken);

            // Fermer DialogWindow
            _notification.CloseDialogWindow();

            // Fermer l'application
            await ForceClose();
        }


        private async Task<bool> IsCloseCommandForSessionAsync()
        {
            return await _qhUserSessionCommand.HandleExistsByCommandTypeUserIdAppIdAsync(_settings.GetCloseCommandType(), _settings.GetAppUserID(), _settings.GetAppID());
        }


        private async Task ForceClose()
        {
            await _chUserSessionCommand.HandleDeleteCloseCommandForSessionAsync(ServiceName, nameof(ForceClose));
            _application.ShutdownApplicationForceClose();
        }
    }
}