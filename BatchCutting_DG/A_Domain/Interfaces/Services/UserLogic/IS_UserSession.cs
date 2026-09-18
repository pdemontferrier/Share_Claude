
namespace BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic
{
    public interface IS_UserSession
    {
        Task OpenUserSessionAsync(int userId, int appId, string sessionText);
        Task CloseUserSessionAsync(int sessionId);
    }
}
