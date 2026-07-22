using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_UserSession : IQ_Generic<UserSession>
    {
        // Requête spécifique :
        Task<List<UserSession>> HandleGetByUserIdAppIdAsync(int userId, int appId);
        Task<int> HandleGetSessionIdAsync(int userId, int appId);
    }
}
