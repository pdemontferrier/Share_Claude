using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_UserApplication : IR_Generic<UserApplication>
    {
        Task<bool> HasUserAccessAppAsync(int appId, int userId);
    }
}