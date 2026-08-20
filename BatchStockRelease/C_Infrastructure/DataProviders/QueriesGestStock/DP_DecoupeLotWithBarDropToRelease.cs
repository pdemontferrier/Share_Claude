using Microsoft.EntityFrameworkCore;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;

namespace BatchStockRelease.C_Infrastructure.DataProviders.QueriesGestStock
{
    public class DP_DecoupeLotWithBarDropToRelease : IR_DecoupeLotWithBarDropToRelease
    {
        private readonly IDbContextFactory<GestStockContext> _contextFactory;
        public DP_DecoupeLotWithBarDropToRelease(IDbContextFactory<GestStockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<DTO_DecoupeLotWithBarDropToRelease>> GetLotsToReleaseAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await (from dl in context.DecoupeLots
                          join db in context.DecoupeBarres on dl.Id equals db.IdDecoupeLot
                          where dl.Inactif == false
                                && dl.OptimChute == true
                                && db.ApproOrigine == "chute"
                                && db.ApproSortieFaite == false
                                && db.ApproInactif == false
                          group db by new { dl.Id, dl.Designation, dl.ApproIdChariot } into g
                          select new DTO_DecoupeLotWithBarDropToRelease
                          {
                              Id = g.Key.Id,
                              Designation = g.Key.Designation,
                              NombreBarres = g.Count(),
                              ApproIdChariot = g.Key.ApproIdChariot,
                              ApproRupture = g.Any(b => b.ApproRupture == true)
                          }).ToListAsync();
        }
    }
}
