using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_CommandeClientModification : QH_Generic<CommandeClientModification>, IQ_CommandeClientModification
    {
        public QH_CommandeClientModification(IR_Generic<CommandeClientModification> repository)
            : base(repository)
        {
        }


        // Requête spécifique :

    }
}