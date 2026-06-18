using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_VieApplication : IR_Generic<VieApplication>
    {
        Task<bool> IsAppAccessibleAsync(int appId);
    }
}