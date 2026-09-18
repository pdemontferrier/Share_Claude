using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_VieChuteMagasinReference : CR_Generic<VieChuteMagasinReference>, IR_VieChuteMagasinReference
    {
        public CR_VieChuteMagasinReference(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }


        // Requête spécifique : Retourne la liste des chutes pour un article interne donné
        public async Task<List<VieChuteMagasinReference>> GetByArticleInterneIdAsync(int articleInterneId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.VieChuteMagasinReferences
                .AsNoTracking()
                .Where(c => c.IdArticleInterne == articleInterneId)
                .OrderBy(c => c.LongueurBarre)
                .ToListAsync();
        }
    }
}