using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_UserDroit : CR_Generic<UserDroit>, IR_UserDroit
    {
        public CR_UserDroit(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Vérifier si une action utilisateur est déclarer pour un utilisateur donné.
        public async Task<bool> HasUserActionAsync(int userId, int actionId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.UserDroits
                .AsNoTracking()
                .AnyAsync(ud => ud.IdUser == userId && ud.IdAction == actionId);
        }
    }
}