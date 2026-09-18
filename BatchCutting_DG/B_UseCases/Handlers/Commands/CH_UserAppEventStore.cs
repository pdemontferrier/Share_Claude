using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using Microsoft.EntityFrameworkCore;

namespace BatchCutting_DG.B_UseCases.Handlers.Commands
{
    public class CH_UserAppEventStore : IC_UserAppEventStore
    {
        private readonly IR_Generic<UserAppEventStore> _repository;
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;

        public CH_UserAppEventStore(IR_Generic<UserAppEventStore> repository, IS_Settings settings, IS_Notification notification)
        {
            _repository = repository;
            _settings = settings;
            _notification = notification;
        }

        // Commande spécifique : Ajouter un enregistrement UserAppEventStore
        public async Task HandleAddAsync(string tableDesignation, int tableId, string data, string appCallChain, string appHandlerCommand, string appCommandMethod)
        {
            try
            {
                var entity = new UserAppEventStore
                {
                    TableDesignation = tableDesignation,
                    TableId = tableId,
                    Timestamp = _settings.GetAppDateTime(),
                    Data = data,
                    AppId = _settings.GetAppID(),
                    AppCallChain = appCallChain,
                    AppHandlerCommand = appHandlerCommand,
                    AppCommandMethod = appCommandMethod,
                    AppUserId = _settings.GetAppUserID(),
                    DeviceUser = _settings.GetAppDeviceUser(),
                    DeviceId = _settings.GetAppDeviceID(),
                    DeviceIp = _settings.GetAppDeviceIP()
                };

                await _repository.AddAsync(entity);
            }
            catch (DbUpdateException ex)
            {
                _notification.Error("No_EC_06", ex.Message);
            }
            catch (ArgumentException ex)
            {
                _notification.Error("No_EC_09", ex.Message);
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_03", ex.Message);
            }
        }
    }
}