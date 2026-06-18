using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeMachine : QH_Generic<DecoupeMachine>, IQ_DecoupeMachine
    {
        public QH_DecoupeMachine(IR_Generic<DecoupeMachine> repository)
            : base(repository)
        {
        }


        // Requête spécifique :

    }
}