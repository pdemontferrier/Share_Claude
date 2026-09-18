using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_CommandeClientActionType : QH_Generic<CommandeClientActionType>, IQ_CommandeClientActionType
    {
        public QH_CommandeClientActionType(IR_Generic<CommandeClientActionType> repository)
            : base(repository)
        {
        }


        // Requête spécifique :

    }
}