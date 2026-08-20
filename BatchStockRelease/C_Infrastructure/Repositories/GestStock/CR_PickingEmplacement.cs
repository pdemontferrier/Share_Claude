using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_PickingEmplacement : CR_Generic<PickingEmplacement>, IR_PickingEmplacement
    {
        public CR_PickingEmplacement(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Retourne la liste des PickingEmplacement commençant par 'Chariot'
        public async Task<List<PickingEmplacement>> GetChariotListAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.PickingEmplacements
                .AsNoTracking()
                .Where(pe => EF.Functions.Like(pe.Nom, "Chariot%"))
                .ToListAsync();
        }
    }
}