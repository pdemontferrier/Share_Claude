using Microsoft.EntityFrameworkCore;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_DecoupeLot : CR_Generic<DecoupeLot>, IR_DecoupeLot
    {
        public CR_DecoupeLot(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Vérifier si le lot a été approvisionné en barre de chute
        public async Task<bool> CheckApproChuteAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            var lotchute = await context.DecoupeLots
                .FirstOrDefaultAsync(dl => dl.Id == decoupeLotId &&
                                           dl.OptimChute == true &&
                                           dl.ApproChute == true);
            return lotchute != null;
        }

        // Requête spécifique : Vérifier si le lot a été approvisionné en barre neuve
        public async Task<bool> CheckApproNeufAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            var lotnew = await context.DecoupeLots
               .FirstOrDefaultAsync(dl => dl.Id == decoupeLotId &&
                                          dl.OptimNeuf == true &&
                                          dl.ApproNeuf == true);
            return lotnew != null;
        }

    }
}