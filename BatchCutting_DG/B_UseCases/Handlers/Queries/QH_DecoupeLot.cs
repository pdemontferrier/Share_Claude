using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeLot : QH_Generic<DecoupeLot>, IQ_DecoupeLot
    {
        public QH_DecoupeLot(IR_Generic<DecoupeLot> repository)
            : base(repository)
        {
        }


        // Requête spécifique :

    }
}