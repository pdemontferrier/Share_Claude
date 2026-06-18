using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeDetail : IQ_Generic<DecoupeDetail>
    {
        // Requête spécifique :
        Task<List<DecoupeDetail>> HandleGetToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId);
        Task<List<DecoupeDetail>> HandleGetArticleComposeToBeAddedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId);
        Task<List<string>> HandleGetCuttingMachineListToBeSuppliedAsync(int decoupeLotId);
        Task<List<int>> HandleGetArticleInterneIdListToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId);
        Task<List<int>> HandleGetArticleComposeIdListToBeAddedAsync(int decoupeLotId, string decoupeMachineId);
        Task<List<DecoupeDetail>> HandleGetAllByDecoupeBarreIdAsync(int decoupeLotId);
        Task<List<DecoupeDetail>> HandleGetIndice1ByLotAsync(int decoupeLotId);
        Task<List<DecoupeDetail>> HandleGetIndice2ByLotAsync(int decoupeLotId);
        Task<List<DecoupeDetail>> HandleGetBatchDecoupeDetailAsync(int decoupeLotId);
        Task<List<string?>> HandleGetBatchCuttingMachineListAsync(int decoupeLotId);
        Task<List<string>> HandleGetCuttingMachineDGListToBeSuppliedAsync(int decoupeLotId);
        Task<List<string?>> HandleGetBatchDecoupeDetailMessageElumatecAsync(int decoupeLotId, string categorie4Value);
    }
}