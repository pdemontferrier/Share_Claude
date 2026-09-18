using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.B_UseCases.Handlers.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.App.DTOs;

namespace BatchStockRelease.B_UseCases.Handlers.Commands
{
    public class CH_UserSession : CH_Generic<UserSession>, IC_UserSession
    {
        private readonly string _callee;
        private readonly IR_Generic<UserSession> _repository;
        private readonly IQ_AppContext _appContext;
        private readonly DTO_AppContext _appCtx;

        public CH_UserSession(
            IR_Generic<UserSession> repository, 
            IC_UserAppEventStore eventStore,
            IQ_AppContext appContext)
            : base(repository, eventStore)
        {
            _callee = GetType().Name;
            _repository = repository;
            _appContext = appContext;

            _appCtx = _appContext.GetAppContext();
        }

        // Commande spécifique : Ajouter un enregistrement à UserSession
        public async Task HandleCreateNewUserSessionAsync(string callChain)
        {
            string handlerCommand = _callee;
            string commandMethod = nameof(HandleCreateNewUserSessionAsync);

            var entity = new UserSession
            {
                IdApplication = _appCtx.AppId,
                IdUser = _appCtx.AppUserId,
                DeviceUser = _appCtx.AppDeviceUser,
                DeviceId = _appCtx.AppDeviceId,
                DeviceIp = _appCtx.AppDeviceIP,
                Connected = true,
                ConnectionDate = _appCtx.AppDateTime,
                DisconnectionDate = _appCtx.AppDateTime
            };

            await HandleAddAsync(entity, callChain, handlerCommand, commandMethod);
        }

        // Commande spécifique : Mettre à jour un enregistrement de UserSession
        public async Task HandleUpdateUserSessionAsync(UserSession entity, bool isConnected, string callChain)
        {
            string handlerCommand = _callee;
            string commandMethod = nameof(HandleUpdateUserSessionAsync);

            entity.DeviceUser = _appCtx.AppDeviceUser;
            entity.DeviceId = _appCtx.AppDeviceId;
            entity.DeviceIp = _appCtx.AppDeviceIP;
            entity.Connected = isConnected;
            if (isConnected)
            {
                entity.ConnectionDate = _appCtx.AppDateTime;
            }
            else
            {
                entity.DisconnectionDate = _appCtx.AppDateTime;
            }

            await HandleUpdateAsync(entity, callChain, handlerCommand, commandMethod);
        }

        // Commande spécifique : Supprime des enregistrements de UserSession
        public async Task HandleDeleteAdditionalSessions(IEnumerable<UserSession> additionalSessions, string callChain)
        {
            string handlerCommand = _callee;
            string commandMethod = nameof(HandleDeleteAdditionalSessions);

            foreach (var entity in additionalSessions)
            {
                await HandleDeleteAsync(entity.Id, callChain, handlerCommand, commandMethod);
            }
        }
    }
}