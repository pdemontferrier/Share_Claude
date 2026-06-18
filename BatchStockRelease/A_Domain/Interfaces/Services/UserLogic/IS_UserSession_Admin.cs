
namespace BatchStockRelease.A_Domain.Interfaces.Services.UserLogic
{
    public interface IS_UserSession_Admin
    {
        Task ListenForCommandsAsync(CancellationToken cancellationToken);
        Task IssueCloseSessionCommandAsync(int targetSessionId);
    }
}
