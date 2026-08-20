using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_CommandeClient : CR_Generic<CommandeClient>, IR_CommandesClient
    {
        public CR_CommandeClient(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Obtenir la liste des Commande Client pour un lot donné
        public async Task<List<CommandeClient>> GetByDecoupeLotIdAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.CommandeClients
                .AsNoTracking()
                .Where(cc => cc.IdDecoupeLot == decoupeLotId)
                .ToListAsync();
        }
    }
}