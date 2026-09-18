using BatchCutting_DG.A_Domain.Entities.GestStock;

namespace BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic
{
    public interface IS_UserAuthentification
    {
        Task<User?> IsLoginPasswordValidAsync(string username, string password);
        Task<bool> AuthenticateWindowsUserAsync();
        Task<User?> GetUserByLoginWindowsAsync(string loginWindows);
        Task CheckUserAccessAppAsync(int userId, int appId, int appAccess, string sessionText);
    }
}