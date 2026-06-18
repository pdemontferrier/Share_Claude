using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    public interface IS_Messages
    {
        Task<List<UserAppMessage>> GetMessagesReceivedAsync(string callChain);
        Task<List<UserAppMessage>> GetMessagesSentAsync(string callChain);
        Task MarkMessageAsReadAsync(int messageId, string callChain);
        Task AddNewMessageAsync(int idAppRecepient, string subject, string content, string callChain);
        Task ListenForCommandsAsync(CancellationToken cancellationToken, string callChain);
    }
}
