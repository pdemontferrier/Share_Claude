using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_UserAppPageDroit : IQ_Generic<UserAppPageDroit>
    {
        // Requête spécifique :
        Task<List<UserAppPageDroit>> HandleGetByUserIdAppIdAsync(int userId, int appId);
    }
}
