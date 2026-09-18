using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_StockModif : QH_Generic<StockModif>, IQ_StockModif
    {
        public QH_StockModif(IR_Generic<StockModif> repository)
            : base(repository)
        {
        }

        // Requête spécifique :


    }
}