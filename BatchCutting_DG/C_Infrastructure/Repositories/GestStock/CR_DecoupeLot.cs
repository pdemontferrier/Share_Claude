using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
{
    public class CR_DecoupeLot : CR_Generic<DecoupeLot>, IR_Generic<DecoupeLot>
    {
        public CR_DecoupeLot(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}