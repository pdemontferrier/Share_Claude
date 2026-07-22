using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeDetail : IR_Generic<DecoupeDetail>
    {
        Task<List<DecoupeDetail>> GetToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId);
        Task<List<DecoupeDetail>> GetArticleComposeToBeAddedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId);
        Task<List<string>> GetCuttingMachineListToBeSuppliedAsync(int decoupeLotId);
        Task<List<int>> GetArticleInterneIdListToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId);
        Task<List<int>> GetArticleComposeIdListToBeAddedAsync(int decoupeLotId, string decoupeMachineId);
        Task<List<DecoupeDetail>> GetAllByDecoupeBarreIdAsync(int decoupeBarreId);
        Task<List<DecoupeDetail>> GetIndice1ByLotAsync(int decoupeLotId);
        Task<List<DecoupeDetail>> GetIndice2ByLotAsync(int decoupeLotId);
        Task<List<DecoupeDetail>> GetBatchDecoupeDetailAsync(int decoupeLotId);
        Task<List<string?>> GetBatchCuttingMachineListAsync(int decoupeLotId);
        Task<List<string>> GetCuttingMachineDGListToBeSuppliedAsync(int decoupeLotId);
        Task<List<string?>> GetBatchDecoupeDetailMessageElumatecAsync(int decoupeLotId, string categorie4Value);
    }
}