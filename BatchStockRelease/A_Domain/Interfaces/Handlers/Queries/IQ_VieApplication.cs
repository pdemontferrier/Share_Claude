using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_VieApplication : IQ_Generic<VieApplication>
    {
        // Requête spécifique :
        Task<bool> HandleGetApplicationAccessibilityAsync(int appId);
    }
}
