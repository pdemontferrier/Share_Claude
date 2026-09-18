using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;
using BatchCutting_DG.B_UseCases.Handlers.Generic;

namespace BatchCutting_DG.B_UseCases.Handlers.Commands
{
    public class CH_ChutesMagasin : CH_Generic<ChutesMagasin>, IC_ChutesMagasin
    {
        public CH_ChutesMagasin(IR_Generic<ChutesMagasin> repository, IC_UserAppEventStore eventStore)
            : base(repository, eventStore)
        {
        }

        // Commande spécifique :

    }
}