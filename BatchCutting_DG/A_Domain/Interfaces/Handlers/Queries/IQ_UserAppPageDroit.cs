using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_UserAppPageDroit : IQ_Generic<UserAppPageDroit>
    {
        // Requête spécifique :
        Task<List<UserAppPageDroit>> HandleGetByUserIdAppIdAsync(int userId, int appId);
    }
}
