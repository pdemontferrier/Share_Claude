using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Commands
{
    public class CH_Stock : CH_Generic<Stock>, IC_Stock
    {
        public CH_Stock(IR_Generic<Stock> repository, IC_UserAppEventStore eventStore)
            : base(repository, eventStore)
        {
        }

        // Commande spécifique :

    }
}