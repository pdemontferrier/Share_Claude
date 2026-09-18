using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.QueryDataProviders.GestStock
{
    public class DP_DecoupeLotWithCut : IR_DecoupeLotWithCut
    {
        private readonly IDbContextFactory<GestStockContext> _contextFactory;
        private readonly IS_Settings _settings;

        public DP_DecoupeLotWithCut(IDbContextFactory<GestStockContext> contextFactory, IS_Settings settings)
        {
            _contextFactory = contextFactory;
            _settings = settings;
        }

        public async Task<List<DTO_DecoupeLotWithCut>> GetDecoupeLotWithCutAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await (from dd in context.DecoupeDetails
                          join dl in context.DecoupeLots on dd.IdDecoupeLot equals dl.Id
                          join db in context.DecoupeBarres on dd.IdDecoupeBarre equals db.Id
                          where dd.DecoupeFaite == false
                            && dd.Inactif == false
                            && dd.ValidationLigne == true
                            && dd.Categorie4 == _settings.GetCuttingMachine()
                            && (dd.ApproOptimBarreChute == true || dd.ApproOptimBarreNeuve == true)
                            && dl.ApproChute == true
                            && dl.ApproNeuf == true
                            && dl.Inactif == false
                            && dl.DecoupeDg == false
                            && db.ApproSortieFaite == true
                          group dd by new { dl.Id, dl.Designation } into g
                          orderby g.Key.Designation
                          select new DTO_DecoupeLotWithCut
                          {
                              Id = g.Key.Id,
                              Designation = g.Key.Designation,
                              NombreDecoupe = g.Count()

                          }).ToListAsync();
        }
    }
}