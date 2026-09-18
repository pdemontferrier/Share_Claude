using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_VieChuteMagasinReference : IR_Generic<VieChuteMagasinReference>
    {
        Task<List<VieChuteMagasinReference>> GetByArticleInterneIdAsync(int articleInterneId);
    }
}