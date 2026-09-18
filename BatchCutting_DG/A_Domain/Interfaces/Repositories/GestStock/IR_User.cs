using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_User : IR_Generic<User>
    {
        Task<User?> GetByLoginAndPasswordAsync(string login, string encryptedPassword);
        Task<User?> GetByLoginWindowsAsync(string loginWindows);
    }
}