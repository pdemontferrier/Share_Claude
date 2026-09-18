using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
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