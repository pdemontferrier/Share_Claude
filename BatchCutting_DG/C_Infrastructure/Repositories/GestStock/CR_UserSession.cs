using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
{
    public class CR_UserSession : CR_Generic<UserSession>, IR_UserSession
    {
        public CR_UserSession(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Obtenir les UserSession par appId et userId
        public async Task<List<UserSession>> GetByUserIdAppIdAsync(int userId, int appId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.UserSessions
                .AsNoTracking()
                .Where(us => us.IdUser == userId && us.IdApplication == appId)
                .ToListAsync();
        }
    }
}