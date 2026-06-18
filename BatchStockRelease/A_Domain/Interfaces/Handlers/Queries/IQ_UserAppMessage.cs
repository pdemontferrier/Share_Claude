using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_UserAppMessage : IQ_Generic<UserAppMessage>
    {
        // Requête spécifique :
        Task<List<UserAppMessage>> HandleGetMessagesReceivedAsync();
        Task<List<UserAppMessage>> HandleGetMessagesSentAsync();
        Task<bool> HandleGetAnyMessageNotReadAsync();
    }
}
