using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeLot : IQ_Generic<DecoupeLot>
    {
        Task<bool> HandleCheckApproChuteAsync(int decoupeLotId);
        Task<bool> HandleCheckApproNeufAsync(int decoupeLotId);
    }
}