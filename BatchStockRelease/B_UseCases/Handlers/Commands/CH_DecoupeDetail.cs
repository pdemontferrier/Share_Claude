using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Commands
{
    public class CH_DecoupeDetail : CH_Generic<DecoupeDetail>, IC_DecoupeDetail
    {
        private readonly string _callee;
        private readonly IR_Generic<DecoupeDetail> _repository;

        public CH_DecoupeDetail(IR_Generic<DecoupeDetail> repository, IC_UserAppEventStore eventStore)
            : base(repository, eventStore)
        {
            _callee = GetType().Name;
            _repository = repository;
        }

        // Commande spécifique : Mettre à jour un ensemble d'enregistrement pour un IdDecoupeBarre donné
        public async Task HandleDisableOptimisationForBarreAsync(int decoupeBarreId, string callChain)
        {
            string handlerCommand = _callee;
            string commandMethod = nameof(HandleDisableOptimisationForBarreAsync);
            var all = await _repository.GetAllAsync();

            var toUpdate = all
                .Where(dd => dd.IdDecoupeBarre == decoupeBarreId)
                .ToList();

            foreach (var dd in toUpdate)
            {
                dd.ApproOptimBarreChute = false;
                dd.ApproOptimBarreNeuve = false;
            }

            await HandleUpdateRangeAsync(toUpdate, callChain, handlerCommand, commandMethod);
        }
    }
}