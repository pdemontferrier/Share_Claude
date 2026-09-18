using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using System.Runtime.CompilerServices;

namespace BatchCutting_DG.B_UseCases.Handlers.Commands
{
    public class CH_UserSession : CH_Generic<UserSession>, IC_UserSession
    {
        private readonly IR_Generic<UserSession> _repository;
        private readonly IS_Settings _settings;

        public CH_UserSession(IR_Generic<UserSession> repository, IC_UserAppEventStore eventStore, IS_Settings settings)
            : base(repository, eventStore)
        {
            _repository = repository;
            _settings = settings;
        }

        // Commande spécifique : Ajouter un enregistrement à UserSession
        public async Task HandleCreateNewUserSessionAsync(string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);

            var entity = new UserSession
            {
                IdApplication = _settings.GetAppID(),
                IdUser = _settings.GetAppUserID(),
                DeviceUser = _settings.GetAppDeviceUser(),
                DeviceId = _settings.GetAppDeviceID(),
                DeviceIp = _settings.GetAppDeviceIP(),
                Connected = true,
                ConnectionDate = _settings.GetAppDateTime(),
                DisconnectionDate = _settings.GetAppDateTime()
            };

            await HandleAddAsync(entity, appService, appServiceCaller);
        }

        // Commande spécifique : Mettre à jour un enregistrement de UserSession
        public async Task HandleUpdateUserSessionAsync(UserSession entity, bool isConnected, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);

            entity.DeviceUser = _settings.GetAppDeviceUser();
            entity.DeviceId = _settings.GetAppDeviceID();
            entity.DeviceIp = _settings.GetAppDeviceIP();
            entity.Connected = isConnected;
            if (isConnected)
            { 
                entity.ConnectionDate = _settings.GetAppDateTime(); 
            }
            else
            {
                entity.DisconnectionDate = _settings.GetAppDateTime();
            }

            await HandleUpdateAsync(entity, appService, appServiceCaller);
        }

        // Commande spécifique : Supprime des enregistrements de UserSession
        public async Task HandleDeleteAdditionalSessions(IEnumerable<UserSession> additionalSessions, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);

            foreach (var entity in additionalSessions)
            {
                await HandleDeleteAsync(entity.Id, appService, appServiceCaller);
            }
        }
    }
}