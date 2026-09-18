using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_UserSessionCommand : IR_Generic<UserSessionCommand>
    {
        Task<bool> ExistsByCommandTypeUserIdAppIdAsync(string commandType, int userId, int appId);
        Task<List<UserSessionCommand>> GetByCommandTypeUserIdAppIdAsync(string commandType, int userId, int appId);
    }
}