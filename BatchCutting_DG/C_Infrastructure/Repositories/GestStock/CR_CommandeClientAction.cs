using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
{
    public class CR_CommandeClientAction : CR_Generic<CommandeClientAction>, IR_Generic<CommandeClientAction>
    {
        public CR_CommandeClientAction(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}