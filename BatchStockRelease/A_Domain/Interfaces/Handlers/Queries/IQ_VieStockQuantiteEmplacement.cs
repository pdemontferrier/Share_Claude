using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_VieStockQuantiteEmplacement : IQ_Generic<VieStockQuantiteEmplacement>
    {
        // Requête spécifique :
        Task<List<VieStockQuantiteEmplacement>> HandleGetByArticleInterneIdAsync(int articleInterneId);
    }
}