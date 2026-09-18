using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_UserAppMessage : IC_Generic<UserAppMessage>
    {
        Task HandleMarkAsReadAsync(int messageId, string callChain);
    }
}
