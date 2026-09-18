using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_VieStockQuantiteEmplacement : CR_Generic<VieStockQuantiteEmplacement>, IR_VieStockQuantiteEmplacement
    {
        public CR_VieStockQuantiteEmplacement(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }


        // Requête spécifique : Retourne la liste des disponibilités deb arre neuves pour un article interne donné
        public async Task<List<VieStockQuantiteEmplacement>> GetByArticleInterneIdAsync(int articleInterneId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.VieStockQuantiteEmplacements
                .AsNoTracking()
                .Where(c => c.IdArticleInterne == articleInterneId)
                .OrderBy(c => c.ZonePriorite)
                .ThenBy(v => v.AdressePriorite)
                .ToListAsync();
        }
    }
}