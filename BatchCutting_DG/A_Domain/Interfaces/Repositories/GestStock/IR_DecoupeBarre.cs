using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeBarre : IR_Generic<DecoupeBarre>
    {
        Task<(int chariotId, string chariotDesignation)> GetChariotInfoForLotAsync(int decoupeLotId);
    }
}