using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_VieStockQuantiteEmplacement : IR_Generic<VieStockQuantiteEmplacement>
    {
        Task<List<VieStockQuantiteEmplacement>> GetByArticleInterneIdAsync(int articleInterneId);
    }
}