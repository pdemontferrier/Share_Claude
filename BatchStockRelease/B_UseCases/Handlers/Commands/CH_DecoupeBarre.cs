using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Commands
{
    public class CH_DecoupeBarre : CH_Generic<DecoupeBarre>, IC_DecoupeBarre
    {
        private readonly string _callee;

        public CH_DecoupeBarre(IR_Generic<DecoupeBarre> repository, IC_UserAppEventStore eventStore)
            : base(repository, eventStore)
        {
            _callee = GetType().Name;
        }


        // Commande spécifique : Mettre à jour un ensemble d'enregistrement de DecoupeBarre pour annuler l'allocation
        public async Task HandleDisableAllocationForBarreAsync(IEnumerable<DecoupeBarre> itemsToReset, string callChain)
        {
            string handlerCommand = _callee;
            string commandMethod = nameof(HandleDisableAllocationForBarreAsync);

            foreach (var db in itemsToReset)
            {
                db.IdStock = 0;
                db.ApproAllocation = false;
                db.ApproZonePriorite = 0;
                db.ApproZoneDesignation = string.Empty;
                db.ApproAdressePriorite = 0;
                db.ApproAdresseDesignation = string.Empty;
                db.ApproConteneur = string.Empty;
                db.ApproTypeConteneur = string.Empty;
                db.ApproTypeContenant = string.Empty;
                db.ApproEmplacement = 0;
                db.ApproEmplacementDesignation = string.Empty;
            }

            await HandleUpdateRangeAsync(itemsToReset, callChain, handlerCommand, commandMethod);
        }
    }
}