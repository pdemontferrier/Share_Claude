using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_UserDroit : IQ_Generic<UserDroit>
    {
        // Requête spécifique :
        Task<bool> HandleGetUserActionAsync(int userId, int actionId);
    }
}
