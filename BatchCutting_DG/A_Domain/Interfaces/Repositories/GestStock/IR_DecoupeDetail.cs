using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeDetail : IR_Generic<DecoupeDetail>
    {
        Task<DecoupeDetail?> GetFirstToCutAsync(int decoupeLotId, string machineId);
        Task<List<DecoupeDetail>> GetAllForDecoupeBarreIdAsync(int decoupeBarreId);
        Task<bool> ExistsByDecoupeBarreIdAsync(int decoupeBarreId);
        Task<List<DecoupeDetail>> GetToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId);
        Task<List<string>> GetCuttingMachineListToBeSuppliedAsync(int decoupeLotId);
        Task<List<int>> GetArticleInterneIdListToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId);
        Task<List<int>> GetArticleComposeIdListToBeAddedAsync(int decoupeLotId, string decoupeMachineId);
        Task<List<DecoupeDetail>> GetIndice1ByLotAsync(int decoupeLotId);
        Task<List<DecoupeDetail>> GetIndice2ByLotAsync(int decoupeLotId);
    }
}