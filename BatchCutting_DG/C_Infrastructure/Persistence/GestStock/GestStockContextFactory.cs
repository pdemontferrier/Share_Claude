using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using Microsoft.EntityFrameworkCore;

namespace BatchCutting_DG.C_Infrastructure.Persistence.GestStock
{
    public class GestStockContextFactory : IDbContextFactory<GestStockContext>
    {
        public readonly IS_Settings _settings;

        public GestStockContextFactory(IS_Settings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public GestStockContext CreateDbContext()
        {
            var connectionString = _settings.GetCredentialsGeststock();
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            var options = new DbContextOptionsBuilder<GestStockContext>()
                .UseMySql(connectionString, serverVersion, mysqlOptions =>
                {
                    // Configuration supplémentaire si nécessaire
                    mysqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                })
                .Options;

            return new GestStockContext(options);
        }
    }
}
