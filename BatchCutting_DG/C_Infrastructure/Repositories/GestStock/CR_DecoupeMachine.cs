using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
{
    public class CR_DecoupeMachine : CR_Generic<DecoupeMachine>, IR_DecoupeMachine
    {
        public CR_DecoupeMachine(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Obtenir un enregistrement par l'adresse IP du PC
        public async Task<DecoupeMachine?> GetByDeviceIpAddressAsync(string ipAddress)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeMachines
                .AsNoTracking()
                .FirstOrDefaultAsync(dm => dm.AdresseIpPc == ipAddress);
        }
    }
}