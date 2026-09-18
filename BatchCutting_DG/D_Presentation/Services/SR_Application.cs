using System.Windows;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;

namespace BatchCutting_DG.D_Presentation.Services
{
    public class SR_Application : IS_Application
    {
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;

        public SR_Application(IS_Settings settings, IS_Notification notification) 
        {
            _settings = settings;
            _notification = notification;
        }

        public void ShutdownApplicationForceClose(string? warningText = null)
        {
            _settings.SetForceClose(true);

            if (!string.IsNullOrEmpty(warningText))
            {
                _notification.Warning(warningText);
            }

            Application.Current.MainWindow?.Close();
        }

        public void ShutdownApplication()
        {
            Application.Current.MainWindow?.Close();
        }
    }
}