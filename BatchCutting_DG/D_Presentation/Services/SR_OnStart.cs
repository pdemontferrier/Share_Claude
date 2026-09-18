using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic;

namespace BatchCutting_DG.D_Presentation.Services
{
    public class SR_OnStart : IS_OnStart
    {
        private readonly IQ_User _qhUser;
        private readonly IQ_UserSession _qhUserSession;
        private readonly IS_UserSessionsAdmin _userSessionAdmin;
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;
        private readonly IS_Dictionary _dictionary;

        public SR_OnStart(IQ_User userQuery, IQ_UserSession qhUserSession,
                              IS_UserSessionsAdmin userSessionAdmin, IS_Settings settings, 
                              IS_Notification notification, IS_Dictionary dictionary)
        {
            _qhUser = userQuery;
            _qhUserSession = qhUserSession;
            _userSessionAdmin = userSessionAdmin;
            _settings = settings;
            _notification = notification;
            _dictionary = dictionary;
        }

        public async Task<bool> ExecuteAsync()
        {
            // Tester la connexion à la base de données
            if (!await TestDatabaseConnectionAsync())
            {
                _notification.Warning("No_Wa_02"); // Avertir l'utilisateur si la base de données est inaccessible
                return false;
            }

            // Vérifier s'il existe une session active pour cet utilisateur sur un autre poste
            var existingSessions = await _qhUserSession.HandleGetByUserIdAppIdAsync(_settings.GetAppUserID(), _settings.GetAppID());
            if (existingSessions.Any(s => s.Connected && s.DeviceId == _settings.GetCRDeviceID()))
            {
                _notification.Warning("No_Wa_15"); // Avertir que l'utilisateur est déjà connecté
                return false;
            }

            // Vérifier si l'application est accessible
            if (!await _userSessionAdmin.CheckAppAccessibleAsync())
            {
                _notification.Warning("No_Wa_03"); // Avertir si l'application est inaccessible
                return false;
            }

            // Mettre à jour les informations utilisateur
            await UserFullNameAsync(_settings.GetAppUserID());

            // Mettre à jour le titre de l'application
            _settings.SetApplicationTitle(_dictionary.GetText("App_Ti_00"));

            return true;
        }

        private async Task<bool> TestDatabaseConnectionAsync()
        {
            try
            {
                // Vérifie si un enregistrement existe dans la table User
                var user = await _qhUser.HandleGetFirstOrDefaultAsync();
                return user != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task UserFullNameAsync(int userId)
        {
            if (userId != 0)
            {
                string? fullName = await _qhUser.HandleGetUserFullNameAsync(userId);

                if (!string.IsNullOrEmpty(fullName))
                {
                    _settings.SetAppUserFullName(fullName);
                }
            }
        }
    }
}