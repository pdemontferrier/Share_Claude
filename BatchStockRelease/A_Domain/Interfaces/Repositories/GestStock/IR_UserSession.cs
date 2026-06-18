using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_UserSession : IR_Generic<UserSession>
    {
        Task<List<UserSession>> GetByUserIdAppIdAsync(int userId, int appId);
    }
}