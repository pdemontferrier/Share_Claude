using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
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