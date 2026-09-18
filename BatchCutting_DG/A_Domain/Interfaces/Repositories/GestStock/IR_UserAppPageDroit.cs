using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_UserAppPageDroit : IR_Generic<UserAppPageDroit>
    {
        Task<List<UserAppPageDroit>> GetByUserIdAppIdAsync(int userId, int appId);
    }
}