using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_DecoupeDetail : IC_Generic<DecoupeDetail>
    {
        Task HandleDisableOptimisationForBarreAsync(int decoupeBarreId, string callChain);
    }
}