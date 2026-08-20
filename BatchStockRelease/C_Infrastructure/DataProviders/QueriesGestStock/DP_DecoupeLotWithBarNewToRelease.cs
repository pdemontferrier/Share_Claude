using Microsoft.EntityFrameworkCore;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;

namespace BatchStockRelease.C_Infrastructure.DataProviders.QueriesGestStock
{
    public class DP_DecoupeLotWithBarNewToRelease : IR_DecoupeLotWithBarNewToRelease
    {
        private readonly IDbContextFactory<GestStockContext> _contextFactory;
        public DP_DecoupeLotWithBarNewToRelease(IDbContextFactory<GestStockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<DTO_DecoupeLotWithBarNewToRelease>> GetLotsToReleaseAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            var query = await (
                from dl in context.DecoupeLots
                join db in context.DecoupeBarres on dl.Id equals db.IdDecoupeLot
                where dl.Inactif == false
                      && dl.OptimNeuf == true
                      && db.ApproOrigine == "neuf"
                      && db.ApproInactif == false
                      && db.ApproSortieFaite == false
                group db by new { dl.Id, dl.Designation, dl.ApproIdChariot } into g
                select new DTO_DecoupeLotWithBarNewToRelease
                {
                    Id = g.Key.Id,
                    Designation = g.Key.Designation,
                    NombreBarres = g.Count(),
                    ApproIdChariot = g.Key.ApproIdChariot,
                    ApproRupture = g.Any(b => b.ApproRupture == true)
                }).ToListAsync();

            return query;
        }

        public async Task<List<DTO_DecoupeLotWithBarNewToRelease>> GetLotsOutOfStockAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            var lots = await (
                from dl in context.DecoupeLots
                join db in context.DecoupeBarres on dl.Id equals db.IdDecoupeLot
                where dl.Inactif == false
                      && dl.OptimNeuf == true
                      && db.ApproOrigine == "neuf"
                      && db.ApproInactif == false
                      && db.ApproAllocation == false
                      && db.ApproRupture == true
                      && db.ApproSortieFaite == false
                group db by new { dl.Id, dl.Designation, dl.ApproIdChariot } into g
                select new DTO_DecoupeLotWithBarNewToRelease
                {
                    Id = g.Key.Id,
                    Designation = g.Key.Designation,
                    ApproIdChariot = g.Key.ApproIdChariot,
                    NombreBarres = g.Count(),
                    ApproRupture = true
                }).ToListAsync();

            return lots;
        }

    }
}