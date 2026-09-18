
namespace BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic
{
    public interface IS_UserSessionsAdmin
    {
        Task ListenForCommandsAsync(CancellationToken cancellationToken);
        Task<bool> CheckAppAccessibleAsync();
        Task IssueCloseSessionCommandAsync(int targetSessionId);
    }
}
