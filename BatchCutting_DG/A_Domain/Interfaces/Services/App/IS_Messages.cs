using BatchCutting_DG.A_Domain.Entities.GestStock;

namespace BatchCutting_DG.A_Domain.Interfaces.Services.App
{
    public interface IS_Messages
    {
        event Action<bool>? UnreadMessagesStatusChanged;
        Task<List<UserAppMessage>> GetMessagesReceivedAsync();
        Task<List<UserAppMessage>> GetMessagesSentAsync();
        Task MarkMessageAsReadAsync(int messageId);
        Task AddNewMessageAsync(int idAppRecepient, string subject, string content);
        Task ListenForCommandsAsync(CancellationToken cancellationToken);
    }
}
