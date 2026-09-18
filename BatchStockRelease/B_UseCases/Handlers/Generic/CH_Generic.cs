using System.Text.Json;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Generic
{
    public class CH_Generic<T> : IC_Generic<T> where T : class
    {
        private readonly IR_Generic<T> _repository;
        private readonly IC_UserAppEventStore _eventStore;

        public CH_Generic(IR_Generic<T> repository, IC_UserAppEventStore eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
        }

        // Commande générique : Ajouter un un enregistrement
        public async Task HandleAddAsync(T entity, string callChain, string? handlerCommand = null, string? commandMethod = null)
        {
            handlerCommand ??= this.GetType().Name;
            commandMethod ??= nameof(HandleAddAsync);

            await _repository.AddAsync(entity);
            await LogEventAsync(entity, callChain, handlerCommand, commandMethod);
        }

        // Commande générique : Mettre à jour un enregistrement
        public async Task HandleUpdateAsync(T entity, string callChain, string? handlerCommand = null, string? commandMethod = null)
        {
            handlerCommand ??= this.GetType().Name;
            commandMethod ??= nameof(HandleUpdateAsync);

            await _repository.UpdateAsync(entity);
            await LogEventAsync(entity, callChain, handlerCommand, commandMethod);
        }

        // Commande générique : Mettre à jour un ensemble d'enregistrements
        public async Task HandleUpdateRangeAsync(IEnumerable<T> entities, string callChain, string? handlerCommand = null, string? commandMethod = null)
        {
            handlerCommand ??= this.GetType().Name;
            commandMethod ??= nameof(HandleUpdateRangeAsync);

            await _repository.UpdateRangeAsync(entities);
            await Task.WhenAll(entities.Select(entity => LogEventAsync(entity, callChain, handlerCommand, commandMethod)));
        }

        // Commande générique : Supprimer un enregistrement avec son Id
        public async Task HandleDeleteAsync(int id, string callChain, string? handlerCommand = null, string? commandMethod = null)
        {
            handlerCommand ??= this.GetType().Name;
            commandMethod ??= nameof(HandleDeleteAsync);

            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(id);
                await LogEventAsync(entity, callChain, handlerCommand, commandMethod);
            }
        }

        // Commande générique : Sauvegarder les changements (utile pour les opérations transactionnelles)
        public async Task HandleSaveChangesAsync()
        {
            await _repository.SaveChangesAsync();
        }


        // Ajout d'un log dans ChutesMagasinAppEventSTore
        public async Task LogEventAsync(T entity, string callChain, string handlerCommand, string commandMethod)
        {
            // ⚠️ Necessite que l'entité est une propriété Id
            var idProperty = typeof(T).GetProperty("Id");
            var id = idProperty?.GetValue(entity) is int value ? value : 0;

            string data = JsonSerializer.Serialize(entity);
            await _eventStore.HandleAddAsync(
                tableDesignation: typeof(T).Name,
                tableId: id,
                data: data,
                appCallChain: callChain,
                appHandlerCommand: handlerCommand,
                appCommandMethod: commandMethod
            );
        }
    }
}