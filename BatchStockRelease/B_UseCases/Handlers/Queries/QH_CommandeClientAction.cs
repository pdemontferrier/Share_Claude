using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_CommandeClientAction : QH_Generic<CommandeClientAction>, IQ_CommandeClientAction
    {
        public QH_CommandeClientAction(IR_Generic<CommandeClientAction> repository)
            : base(repository)
        {
        }


        // Requête spécifique :

    }
}