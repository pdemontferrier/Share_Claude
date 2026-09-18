using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;

namespace BatchStockRelease.C_Infrastructure.DataProviders.QueriesApp
{
    public class DP_AppContext : IQ_AppContext
    {
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_User _settingsUser;

        public DP_AppContext(
            IS_Settings_App settingsApp,
            IS_Settings_User settingsUser)
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