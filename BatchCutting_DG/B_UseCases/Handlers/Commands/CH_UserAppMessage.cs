using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using System.Runtime.CompilerServices;

namespace BatchCutting_DG.B_UseCases.Handlers.Commands
{
    public class CH_UserAppMessage : CH_Generic<UserAppMessage>, IC_UserAppMessage
    {
        private readonly IR_Generic<UserAppMessage> _repository;
        private readonly IC_UserAppEventStore _eventStore;

        public CH_UserAppMessage(IR_Generic<UserAppMessage> repository, IC_UserAppEventStore eventStore)
            : base(repository, eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        // Commande spécifique : Mettre à jour IsRead à true pour un message spécifique
        public async Task HandleMarkAsReadAsync(int messageId, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);

            // Récupérer le message correspondant
            var entity = await _repository.GetByIdAsync(messageId);
            if (entity != null)
            {
                entity.IsRead = true;
                await _repository.UpdateAsync(entity);

                // Enregistrer l'événement
                await LogEventAsync(entity, appService, appServiceCaller);
            }
        }
    }
}