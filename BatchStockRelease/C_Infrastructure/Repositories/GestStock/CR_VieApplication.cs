using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_VieApplication : CR_Generic<VieApplication>, IR_VieApplication
    {
        public CR_VieApplication(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Tester si l'application est accéssible
        public async Task<bool> IsAppAccessibleAsync(int appId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.VieApplications
                .AsNoTracking()
                .AnyAsync(va => va.Id == appId && va.Accessible == true);
        }
    }
}