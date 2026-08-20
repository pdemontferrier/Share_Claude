using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_ChutesMagasin : CR_Generic<ChutesMagasin>, IR_ChutesMagasin
    {
        public CR_ChutesMagasin(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Retourne la liste des chutes pour un article interne donné
        public async Task<List<ChutesMagasin>> GetByArticleInterneIdAsync(int articleInterneId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.ChutesMagasins
                .AsNoTracking()
                .Where(c => c.IdArticleInterne == articleInterneId)
                .OrderBy(c => c.Longueur)
                .ToListAsync();
        }

        // Requête spécifique : Retourne la liste des chutes pour un article interne donné
        public async Task<ChutesMagasin?> GetByQrCodeAsync(string qrCode)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.ChutesMagasins
                .AsNoTracking()
                .Where(c => c.CodeBarre == qrCode)
                .FirstOrDefaultAsync();
        }
    }
}