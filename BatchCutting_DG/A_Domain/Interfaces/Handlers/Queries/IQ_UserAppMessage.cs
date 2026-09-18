using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_UserAppMessage : IQ_Generic<UserAppMessage>
    {
        // Requête spécifique :
        Task<List<UserAppMessage>> HandleGetMessagesReceivedAsync();
        Task<List<UserAppMessage>> HandleGetMessagesSentAsync();
        Task<bool> HandleGetAnyMessageNotReadAsync();
    }
}
