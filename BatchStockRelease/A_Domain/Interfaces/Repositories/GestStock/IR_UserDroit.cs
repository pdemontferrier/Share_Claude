using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_UserDroit : IR_Generic<UserDroit>
    {
        Task<bool> HasUserActionAsync(int userId, int actionId);
    }
}