using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.B_UseCases.Handlers.Generic;
using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;

namespace BatchStockRelease.B_UseCases.Handlers.Commands
{
    public class CH_UserSessionCommand : CH_Generic<UserSessionCommand>, IC_UserSessionCommand
    {
        private readonly string _callee;
        private readonly IR_Generic<UserSessionCommand> _repository;
        private readonly IS_Settings_User _settingsUser;
        private readonly IQ_AppContext _appContext;
        private readonly DTO_AppContext _appCtx;

        public CH_UserSessionCommand(
            IR_Generic<UserSessionCommand> repository, 
            IC_UserAppEventStore eventStore, 
            IS_Settings_User settingsUser,
            IQ_AppContext appContext)
            : base(repository, eventStore)
        {
            _callee = GetType().Name;
            _repository = repository;
            _settingsUser = settingsUser;
            _appContext = appContext;

            _appCtx = _appContext.GetAppContext();
        }

        // Commande spécifique : Ajouter un nouvel enregistrement à UserSessionCommand
        public async Task HandleAddCloseSessionCommandAsync(int targetSessionId, string callChain)
        {
            string handlerCommand = _callee;
            string commandMethod = nameof(HandleAddCloseSessionCommandAsync);

            var entity = new UserSessionCommand
            {
                IdUserTarget = targetSessionId,
                IdAppTarget = _appCtx.AppId,
                IdUserIssuer = _appCtx.AppUserId,
                CommandType = _settingsUser.GetCloseCommandType(),
                CommandDate = _appCtx.AppDateTime
            };

            await HandleAddAsync(entity, callChain, handlerCommand, commandMethod);
        }

        // Commande spécifique : Supprimer des enregistrements de UserSessionCommand
        public async Task HandleDeleteCloseCommandForSessionAsync(string callChain)
        {
            string handlerCommand = _callee;
            string commandMethod = nameof(HandleDeleteCloseCommandForSessionAsync);

            // Récupérer les commandes à supprimer via le repository
            var commands = await _repository.GetAllAsync();

            var filteredCommands = commands
                .Where(cmd => cmd.CommandType == _settingsUser.GetCloseCommandType() && cmd.IdUserTarget == _appCtx.AppUserId)
                .ToList();

            if (filteredCommands.Any())
            {
                foreach (var entity in filteredCommands)
                {
                    await HandleDeleteAsync(entity.Id, callChain, handlerCommand, commandMethod);
                }
            }
        }
    }
}