using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;

namespace BatchCutting_DG.B_UseCases.Services.UserLogic
{
    public class SR_UserAccess : IS_UserSettings
    {
        private readonly IQ_UserAppPageDroit _qhUserAppPageDroit;
        private readonly IS_Settings _settings;

        public SR_UserAccess(IQ_UserAppPageDroit uapaQuery, IS_Settings settings)
        {
            _qhUserAppPageDroit = uapaQuery;
            _settings = settings;
        }

        public async Task InitializeUserAccesses()
        {
            // Initialise toutes les pages avec les valeurs par défaut à false pour PagesAccessRights.
            _settings.InitializeUserDefaultAccesses();

            // Met à jour les droits définis dans la base de données
            await UpdatePageAccessRights(_settings.GetAppUserID(), _settings.GetAppID());
        }

        private async Task UpdatePageAccessRights(int userId, int appId)
        {
            // Récupére toutes les pages accessibles et leurs droits
            var accessiblePages = await _qhUserAppPageDroit.HandleGetByUserIdAppIdAsync(userId, appId);

            if (accessiblePages != null && accessiblePages.Any())
            {
                _settings.SetPageAccessRights(accessiblePages);
            }
        }
    }
}