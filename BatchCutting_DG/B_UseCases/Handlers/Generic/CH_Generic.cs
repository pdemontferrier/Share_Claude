using System.Runtime.CompilerServices;
using System.Text.Json;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.B_UseCases.Handlers.Generic
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
        public async Task HandleAddAsync(T entity, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);

            await _repository.AddAsync(entity);
            await LogEventAsync(entity, appService, appServiceCaller, nameof(HandleAddAsync));
        }

        // Commande générique : Mettre à jour un enregistrement
        public async Task HandleUpdateAsync(T entity, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);

            await _repository.UpdateAsync(entity);
            await LogEventAsync(entity, appService, appServiceCaller, nameof(HandleUpdateAsync));
        }

        // Commande générique : Mettre à jour un ensemble d'enregistrements
        public async Task HandleUpdateRangeAsync(IEnumerable<T> entities, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);

            await _repository.UpdateRangeAsync(entities);
            await Task.WhenAll(entities.Select(entity => LogEventAsync(entity, appService, appServiceCaller, nameof(HandleUpdateRangeAsync))));
        }

        // Commande générique : Supprimer un enregistrement avec son Id
        public async Task HandleDeleteAsync(int id, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);

            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(id);
                await LogEventAsync(entity, appService, appServiceCaller, nameof(HandleDeleteAsync));
            }
        }

        // Commande générique : Sauvegarder les changements (utile pour les opérations transactionnelles)
        public async Task HandleSaveChangesAsync()
        {
            await _repository.SaveChangesAsync();
        }


        // Ajout d'un log dans ChutesMagasinAppEventSTore
        public async Task LogEventAsync(T entity, string callChain, string handlerCommand, string? commandMethod = null)
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
                appCommandMethod: commandMethod ?? string.Empty
            );
        }


        public string BuildAppServiceCaller(string explicitCaller, string actualCaller)
        {
            if (string.IsNullOrWhiteSpace(explicitCaller))
                return actualCaller;

            if (explicitCaller == actualCaller)
                return actualCaller;

            return $"{explicitCaller} > {actualCaller}";
        }
    }
}