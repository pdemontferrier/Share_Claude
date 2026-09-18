using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Commands
{
    public class CH_CommandeClientStatut : CH_Generic<CommandeClientStatut>, IC_CommandeClientStatut
    {
        public CH_CommandeClientStatut(IR_Generic<CommandeClientStatut> repository, IC_UserAppEventStore eventStore)
            : base(repository, eventStore)
        {
        }

        // Commande spécifique :

    }
}