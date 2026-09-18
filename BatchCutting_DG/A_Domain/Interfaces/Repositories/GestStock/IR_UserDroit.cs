using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_UserDroit : IR_Generic<UserDroit>
    {
        Task<bool> HasUserActionAsync(int userId, int actionId);
    }
}