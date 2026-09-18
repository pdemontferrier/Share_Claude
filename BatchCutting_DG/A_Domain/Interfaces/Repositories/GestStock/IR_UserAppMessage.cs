using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_UserAppMessage : IR_Generic<UserAppMessage>
    {
        Task<List<UserAppMessage>> GetReceivedMessagesAsync(int appId);
        Task<List<UserAppMessage>> GetSentMessagesAsync(int appId);
        Task<bool> HasUnreadMessagesAsync(int appId);
    }
}