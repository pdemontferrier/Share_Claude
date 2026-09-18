using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_VieChuteMagasinReference : IR_Generic<VieChuteMagasinReference>
    {
        Task<List<VieChuteMagasinReference>> GetByArticleInterneIdAsync(int articleInterneId);
    }
}