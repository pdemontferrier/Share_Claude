using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_ChutesMagasin : IR_Generic<ChutesMagasin>
    {
        Task<List<ChutesMagasin>> GetByArticleInterneIdAsync(int articleInterneId);
    }
}