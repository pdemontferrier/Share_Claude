using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_UserSessionCommand : CR_Generic<UserSessionCommand>, IR_UserSessionCommand
    {
        public CR_UserSessionCommand(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Vérifier l'existence de UserSessionCommand par commandType, UserId et AppId
        public async Task<bool> ExistsByCommandTypeUserIdAppIdAsync(string commandType, int userId, int appId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.UserSessionCommands
                .AsNoTracking()
                .AnyAsync(usc => usc.CommandType == commandType
                                 && usc.IdUserTarget == userId
                                 && usc.IdAppTarget == appId);
        }

        // Requête spécifique : Obtenir les UserSessionCommand par commandType, UserId et AppId
        public async Task<List<UserSessionCommand>> GetByCommandTypeUserIdAppIdAsync(string commandType, int userId, int appId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.UserSessionCommands
                .AsNoTracking()
                .Where(usc => usc.CommandType == commandType
                              && usc.IdUserTarget == userId
                              && usc.IdAppTarget == appId)
                .ToListAsync();
        }
    }
}