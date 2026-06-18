using Microsoft.EntityFrameworkCore;
using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;

namespace BatchStockRelease.B_UseCases.Handlers.Commands
{
    public class CH_UserAppEventStore : IC_UserAppEventStore
    {
        private readonly IR_Generic<UserAppEventStore> _repository;
        private readonly IQ_AppContext _appContext;
        private readonly IS_Notification _notification;

        public CH_UserAppEventStore(
            IR_Generic<UserAppEventStore> repository, 
            IQ_AppContext appContext, 
            IS_Notification notification)
        {
            _repository = repository;
            _appContext = appContext;
            _notification = notification;
        }

        // Commande spécifique : Ajouter un enregistrement UserAppEventStore
        public async Task HandleAddAsync(string tableDesignation, int tableId, string data, string appCallChain, string appHandlerCommand, string appCommandMethod)
        {
            try
            {
                // Eléments d'identification de l'application'
                DTO_AppContext appCtx = _appContext.GetAppContext();

                var entity = new UserAppEventStore
                {
                    TableDesignation = tableDesignation,
                    TableId = tableId,
                    Timestamp = appCtx.AppDateTime,
                    Data = data,
                    AppId = appCtx.AppId,
                    AppCallChain = appCallChain,
                    AppHandlerCommand = appHandlerCommand,
                    AppCommandMethod = appCommandMethod,
                    AppUserId = appCtx.AppUserId,
                    DeviceUser = appCtx.AppDeviceUser,
                    DeviceId = appCtx.AppDeviceId,
                    DeviceIp = appCtx.AppDeviceIP
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