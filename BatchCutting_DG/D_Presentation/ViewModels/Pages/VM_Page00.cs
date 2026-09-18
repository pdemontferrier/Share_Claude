using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page00
    {
        private readonly IS_UserAuthentification _userAuthentification;
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;
        private readonly IS_Application _application;
        private readonly IS_Dictionary _dictionary;

        public VM_Page00(IS_UserAuthentification userAuthentification, IS_Settings settings, 
                               IS_Notification notification, IS_Application application,
                               IS_Dictionary dictionary)
        {
            _userAuthentification = userAuthentification;
            _settings = settings;
            _notification = notification;
            _application = application;
            _dictionary = dictionary;

            LoadData();
        }

        private void LoadData()
        {
            // A Définir
        }

        public async Task UserAuthenticationAsync( string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                UserAttempt("No_Se_01");
                return;
            }

            var user = await _userAuthentification.IsLoginPasswordValidAsync(login, password);
            if (user != null)
            {
                // Vérifier si l'utilisateur peut accéder à l'application
                await _userAuthentification.CheckUserAccessAppAsync(user.Id, _settings.GetAppID(), _settings.GetAppAccess(), _dictionary.GetText("P00_05"));
            }
            else
            {
                UserAttempt("No_Wa_05");
            }
        }

        private void UserAttempt(string messageText)
        {
            _settings.IncrementUserAttempt();
            if (_settings.GetUserAttempt() >= 3)
            {
                _application.ShutdownApplicationForceClose("No_Wa_07");
            }
            else
            {
                _notification.Warning(messageText, $"\n{_settings.GetUserAttempt()} {_dictionary.GetText("No_Wa_06")}");
            }
        }
    }
}