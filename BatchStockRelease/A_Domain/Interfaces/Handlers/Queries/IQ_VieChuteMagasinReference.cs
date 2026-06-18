using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_VieChuteMagasinReference : IQ_Generic<VieChuteMagasinReference>
    {
        // Requête spécifique :
        Task<List<VieChuteMagasinReference>> HandleGetByArticleInterneIdAsync(int articleInterneId);
    }
}