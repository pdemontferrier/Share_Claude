using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Commands
{
    public class CH_ChutesArchive : CH_Generic<ChutesArchive>, IC_ChutesArchive
    {
        public CH_ChutesArchive(IR_Generic<ChutesArchive> repository, IC_UserAppEventStore eventStore)
            : base(repository, eventStore)
        {
        }

        // Commande spécifique :

    }
}