using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchCutting_DG.C_Infrastructure.Services.GestStock
{
    public class SR_DataBase : IS_DataBase
    {
        private readonly IDbContextFactory<GestStockContext> _contextFactory;

        public SR_DataBase(IDbContextFactory<GestStockContext>? contextFactory = null)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        private async Task ExecuteProcedureAsync(string procedureName)
        {
            using var context = _contextFactory.CreateDbContext();
            var sql = $"CALL {procedureName}();";
            await context.Database.ExecuteSqlRawAsync(sql);
        }

        private async Task ExecuteProcedureArg1Async(string procedureName, string parameter)
        {
            using var context = _contextFactory.CreateDbContext();
            var sql = $"CALL {procedureName}('{parameter}');";
            await context.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task SprApplicationAccessUpdateAsync(int appID)
        {
            await ExecuteProcedureArg1Async("spr_applications_update", appID.ToString());
        }

        public async Task SprDecoupeBarreUpdateEmpSCAsync(int appID)
        {
            await ExecuteProcedureArg1Async("spr_decoupe_barre_update_emp_SC", appID.ToString());
        }

        public async Task SprCommandeClientProductionUpdateAsync()
        {
            await ExecuteProcedureAsync("spr_commande_client_production_update");
        }

    }
}
