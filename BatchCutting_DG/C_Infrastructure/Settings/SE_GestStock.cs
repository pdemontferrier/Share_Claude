using Microsoft.EntityFrameworkCore;
using CommonResources.Settings;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.B_UseCases.Settings;

namespace BatchCutting_DG.C_Infrastructure.Settings
{
    public static class SE_GestStock
    {
        // Acces base de données
        public static readonly string Credential = SE_App.Environment == "Prod" ? CR_DataBaseSettings.CredentialsGeststock_prod : CR_DataBaseSettings.CredentialsGeststock_dev;
    }

    public class GestStockContextFactory : IDbContextFactory<GestStockContext>
    {
        public GestStockContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<GestStockContext>()
                .UseMySql(SE_GestStock.Credential, ServerVersion.AutoDetect(SE_GestStock.Credential))
                .Options;
            return new GestStockContext(options);
        }
    }
}
