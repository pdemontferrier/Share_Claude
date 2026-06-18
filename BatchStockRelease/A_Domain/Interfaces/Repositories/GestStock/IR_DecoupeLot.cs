using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;


namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeLot : IR_Generic<DecoupeLot>
    {
        Task<bool> CheckApproChuteAsync(int decoupeLotId);
        Task<bool> CheckApproNeufAsync(int decoupeLotId);
    }
}
