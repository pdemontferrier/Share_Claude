using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_UserAppPageDroit : CR_Generic<UserAppPageDroit>, IR_UserAppPageDroit
    {
        public CR_UserAppPageDroit(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Obtenir le premier UserAppPageAccess en fonction de userId, appId
        public async Task<List<UserAppPageDroit>> GetByUserIdAppIdAsync(int userId, int appId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.UserAppPageDroits
                .AsNoTracking()
                .Where(record => record.IdUser == userId && record.IdApp == appId)
                .OrderBy(record => record.Page)
                .ToListAsync();
        }
    }
}