using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using System.Runtime.CompilerServices;

namespace BatchCutting_DG.B_UseCases.Handlers.Commands
{
    public class CH_UserSessionCommand : CH_Generic<UserSessionCommand>, IC_UserSessionCommand
    {
        private readonly IR_Generic<UserSessionCommand> _repository;
        private readonly IS_Settings _settings;

        public CH_UserSessionCommand(IR_Generic<UserSessionCommand> repository, IC_UserAppEventStore eventStore, IS_Settings settings)
            : base(repository, eventStore)
        {
            _repository = repository;
            _settings = settings;
        }

        // Commande spécifique : Ajouter un nouvel enregistrement à UserSessionCommand
        public async Task HandleAddCloseSessionCommandAsync(int targetSessionId, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);

            var entity = new UserSessionCommand
            {
                IdUserTarget = targetSessionId,
                IdAppTarget = _settings.GetAppID(),
                IdUserIssuer = _settings.GetAppUserID(),
                CommandType = _settings.GetCloseCommandType(),
                CommandDate = _settings.GetAppDateTime()
            };

            await HandleAddAsync(entity, appService, appServiceCaller);
        }

        // Commande spécifique : Supprimer des enregistrements de UserSessionCommand
        public async Task HandleDeleteCloseCommandForSessionAsync(string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);

            // Récupérer les commandes à supprimer via le repository
            var commands = await _repository.GetAllAsync();

            var filteredCommands = commands
                .Where(cmd => cmd.CommandType == _settings.GetCloseCommandType() && cmd.IdUserTarget == _settings.GetAppUserID())
                .ToList();

            if (filteredCommands.Any())
            {
                foreach (var entity in filteredCommands)
                {
                    await HandleDeleteAsync(entity.Id, appService, appServiceCaller);
                }
            }
        }
    }
}