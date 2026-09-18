using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;

namespace BatchCutting_DG.B_UseCases.Services.UserLogic
{
    public class SR_UserAuthentification : IS_UserAuthentification
    {
        private readonly IQ_User _qhUser;
        private readonly IQ_UserDroit _qhUserDroit;
        private readonly IQ_UserSession _qhUserSession;
        private readonly IS_Settings _settings;
        private readonly IS_UserSettings _userSettings;
        private readonly IS_UserSession _userSession;
        private readonly IS_Navigation _navigation;
        private readonly IS_Application _application;

        public SR_UserAuthentification(IQ_User uQuery, IQ_UserDroit udQuery, IQ_UserSession qhUserSession,
                                           IS_UserSession userSession, IS_Settings settings,
                                           IS_Navigation navigation, IS_Application application,
                                           IS_UserSettings userSettings)
        {
            _qhUser = uQuery;
            _qhUserDroit = udQuery;
            _qhUserSession = qhUserSession;
            _userSession = userSession;
            _settings = settings;
            _navigation = navigation;
            _application = application;
            _userSettings = userSettings;
        }


        public async Task<User?> IsLoginPasswordValidAsync(string loginId, string password)
        {
            return await _qhUser.HandleGetSingleAsync(loginId, password);
        }

        public async Task<bool> AuthenticateWindowsUserAsync()
        {
            // Récupérer le login Windows
            var loginWindows = _settings.GetCRDeviceUser();

            // Récupérer l'utilisateur correspondant
            var user = await GetUserByLoginWindowsAsync(loginWindows);

            // Vérifier si l'utilisateur existe
            if (user != null)
            {
                // Mettre à jour l'ID utilisateur dans les paramètres
                _settings.SetAppUserID(user.Id);
                return true; // Indique que l'utilisateur est authentifié
            }

            return false; // L'utilisateur n'existe pas
        }

        public async Task<User?> GetUserByLoginWindowsAsync(string loginWindows)
        {
            return await _qhUser.HandleGetByLoginWindowsAsync(loginWindows);
        }


        public async Task CheckUserAccessAppAsync(int userId, int appId, int appAccess, string sessionText)
        {
            // Étape 1 : Vérifier si l'utilisateur est déjà connecté ailleurs
            var existingSessions = await _qhUserSession.HandleGetByUserIdAppIdAsync(userId, appId);

            if (existingSessions.Any(s => s.Connected && s.DeviceId == _settings.GetCRDeviceID()))
            {
                _application.ShutdownApplicationForceClose("No_Wa_15");
            }
            else
            {
                // Étape 2 : Lancer la vérification des droits
                bool access = await CanUserAccessAppAsync(userId, appId, appAccess, sessionText);

                // Étape 3 : Si indentifié, afficher la Page10, sinon fermer l'application
                if (access)
                {
                    // Afficher la bannière avant navigation
                    if (System.Windows.Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.DisplayButtonBanner();
                    }

                    // Afficher la Page10
                    _navigation.NavigateToNewPage("Page10");
                }
                else
                {
                    _application.ShutdownApplicationForceClose("No_Wa_04");
                }
            }
        }

        private async Task<bool> CanUserAccessAppAsync(int userId, int appId, int appAccess, string sessionText)
        {
            // Étape 1 : Vérifier les droits
            bool access = await _qhUserDroit.HandleGetUserActionAsync(userId, appAccess);
            _settings.SetCanUserAccessApp(access);

            if (!_settings.GetCanUserAccessApp())
                return false;

            // Étape 2 : Mise à jour des infos utilisateur
            await SetAppUserInfo(userId);

            // Étape 3 : Initialiser les droits
            await _userSettings.InitializeUserAccesses();

            // Étape 4 : Ouvrir une session
            await _userSession.OpenUserSessionAsync(userId, appId, sessionText);

            return true;
        }

        private async Task SetAppUserInfo(int userId)
        {
            _settings.SetAppUserID(userId);
            _settings.SetAppUserFullName(await _qhUser.HandleGetUserFullNameAsync(userId));
            _settings.SetAppDeviceID();
            _settings.SetAppDeviceIP();
            _settings.SetAppDeviceUser();
        }
    }
}