using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_UserApplication : CR_Generic<UserApplication>, IR_UserApplication
    {
        public CR_UserApplication(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Vérifier si un utilisateur à accès à une application
        public async Task<bool> HasUserAccessAppAsync(int appId, int userId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.UserApplications
                .AsNoTracking()
                .AnyAsync(ud => ud.IdUtilisateur == userId && ud.IdApp == appId);
        }
    }
}