using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;
using BatchCutting_DG.B_UseCases.Handlers.Generic;

namespace BatchCutting_DG.B_UseCases.Handlers.Commands
{
    public class CH_DecoupeLot : CH_Generic<DecoupeLot>, IC_DecoupeLot
    {
        public CH_DecoupeLot(IR_Generic<DecoupeLot> repository, IC_UserAppEventStore eventStore)
            : base(repository, eventStore)
        {
        }

        // Commande spécifique :

    }
}