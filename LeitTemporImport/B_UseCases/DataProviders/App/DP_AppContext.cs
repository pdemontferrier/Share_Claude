using LeitTemporImport.A_Domain.DTOs.App;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Settings.App;
using LeitTemporImport.A_Domain.Interfaces.Settings.User;

namespace LeitTemporImport.C_Infrastructure.DataProviders.QueriesApp
{
    public class DP_AppContext : IQ_AppContext
    {
        private readonly ISE_App _settingsApp;
        private readonly ISE_User _settingsUser;

        public DP_AppContext(
            ISE_App settingsApp,
            ISE_User settingsUser)
        {
            _settingsApp = settingsApp;
            _settingsUser = settingsUser;
        }

        public DTO_AppContext GetAppContext()
        {
            return new DTO_AppContext
            {
                AppId = _settingsApp.GetAppId(),
                AppDate = _settingsApp.GetAppDate(),
                AppDateTime = _settingsApp.GetAppDateTime(),
                AppUserId = _settingsUser.GetAppUserId(),
                AppDeviceUser = _settingsUser.GetAppDeviceUser(),
                AppDeviceId = _settingsUser.GetAppDeviceId(),
                AppDeviceIP = _settingsUser.GetAppDeviceIP(),
            };
        }
    }
}