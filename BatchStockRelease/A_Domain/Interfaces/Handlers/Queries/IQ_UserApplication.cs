using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_UserApplication : IQ_Generic<UserApplication>
    {
        // Requête spécifique :
        Task<bool> HandleHasUserAccessAppAsync(int appId, int userId);
    }
}
