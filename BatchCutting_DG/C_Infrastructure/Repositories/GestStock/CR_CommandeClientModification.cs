using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
{
    public class CR_CommandeClientModification : CR_Generic<CommandeClientModification>, IR_Generic<CommandeClientModification>
    {
        public CR_CommandeClientModification(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}