using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_User : CR_Generic<User>, IR_User
    {
        public CR_User(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Obtenir un enregistrement par nom d'utilisateur et mot de passe
        public async Task<User?> GetByLoginAndPasswordAsync(string login, string encryptedPassword)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == login && u.MotDePasse == encryptedPassword);
        }

        // Requête spécifique : Vérifier si le login Windows existe
        public async Task<User?> GetByLoginWindowsAsync(string loginWindows)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.LoginWindows == loginWindows);
        }
    }
}