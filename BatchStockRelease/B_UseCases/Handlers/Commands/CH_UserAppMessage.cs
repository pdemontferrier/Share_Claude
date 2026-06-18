using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Commands
{
    public class CH_UserAppMessage : CH_Generic<UserAppMessage>, IC_UserAppMessage
    {
        private readonly string _callee;
        private readonly IR_Generic<UserAppMessage> _repository;
        private readonly IC_UserAppEventStore _eventStore;

        public CH_UserAppMessage(IR_Generic<UserAppMessage> repository, IC_UserAppEventStore eventStore)
            : base(repository, eventStore)
        {
            _callee = GetType().Name;
            _repository = repository;
            _eventStore = eventStore;
        }

        // Commande spécifique : Mettre à jour IsRead à true pour un message spécifique
        public async Task HandleMarkAsReadAsync(int messageId, string callChain)
        {
            string handlerCommand = _callee;
            string commandMethod = nameof(HandleMarkAsReadAsync);
            var entity = await _repository.GetByIdAsync(messageId);

            if (entity != null)
            {
                entity.IsRead = true;
                await _repository.UpdateAsync(entity);

                // Enregistrer l'événement
                await LogEventAsync(entity, callChain, handlerCommand, commandMethod);
            }
        }
    }
}