using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
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