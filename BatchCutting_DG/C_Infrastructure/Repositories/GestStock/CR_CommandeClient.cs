using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
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