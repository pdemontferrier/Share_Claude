
namespace BatchStockRelease.A_Domain.Interfaces.Services.UserLogic
{
    public interface IS_UserSession_Open
    {
        Task ExecuteAsync(int userId, int appId, string caller);
    }
}
