using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
{
    public class CR_DecoupeBarre : CR_Generic<DecoupeBarre>, IR_DecoupeBarre
    {
        public CR_DecoupeBarre(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Retourne les informations relatives au chariot utilisé pour un lot donné
        public async Task<(int chariotId, string chariotDesignation)> GetChariotInfoForLotAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            var chariotInfo = await context.DecoupeBarres
                .AsNoTracking()
                .Where(db =>
                    db.IdDecoupeLot == decoupeLotId &&
                    db.ApproIdChariot != 0 &&
                    db.ApproChariotDesignation != null)
                .Select(d => new { d.ApproIdChariot, d.ApproChariotDesignation })
                .FirstOrDefaultAsync();

            if (chariotInfo == null)
                return (0, string.Empty);

            return (chariotInfo.ApproIdChariot, chariotInfo.ApproChariotDesignation!);
        }
    }
}