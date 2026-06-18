using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_UserAppMessage : IR_Generic<UserAppMessage>
    {
        Task<List<UserAppMessage>> GetReceivedMessagesAsync(int appId);
        Task<List<UserAppMessage>> GetSentMessagesAsync(int appId);
        Task<bool> HasUnreadMessagesAsync(int appId);
    }
}