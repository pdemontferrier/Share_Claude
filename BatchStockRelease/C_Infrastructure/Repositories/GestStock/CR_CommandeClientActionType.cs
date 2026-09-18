using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_CommandeClientActionType : CR_Generic<CommandeClientActionType>, IR_Generic<CommandeClientActionType>
    {
        public CR_CommandeClientActionType(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }
    }
}