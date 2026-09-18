using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
{
    public class CR_CommandeClientActionType : CR_Generic<CommandeClientActionType>, IR_Generic<CommandeClientActionType>
    {
        public CR_CommandeClientActionType(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}