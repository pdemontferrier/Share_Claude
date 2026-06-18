using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_ChutesArchive : QH_Generic<ChutesArchive>, IQ_ChutesArchive
    {
        public QH_ChutesArchive(IR_Generic<ChutesArchive> repository)
            : base(repository)
        {
        }

        // Requête spécifique :


    }
}