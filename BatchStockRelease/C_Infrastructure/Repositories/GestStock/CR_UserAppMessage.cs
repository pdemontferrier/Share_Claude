using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_UserAppMessage : CR_Generic<UserAppMessage>, IR_UserAppMessage
    {
        public CR_UserAppMessage(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Obtenir la liste des messages reçu
        public async Task<List<UserAppMessage>> GetReceivedMessagesAsync(int appId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserAppMessages
                .AsNoTracking()
                .Where(m => m.IdAppRecepient == appId)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();
        }

        // Requête spécifique : Obtenir la liste des messages envoyés
        public async Task<List<UserAppMessage>> GetSentMessagesAsync(int appId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserAppMessages
                .AsNoTracking()
                .Where(m => m.IdAppSender == appId)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();
        }

        // Requête spécifique : Vérifier s'il existe des messages non lus
        public async Task<bool> HasUnreadMessagesAsync(int appId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.UserAppMessages
                .AsNoTracking()
                .AnyAsync(m => m.IdAppRecepient == appId && m.IsRead == false);
        }
    }
}