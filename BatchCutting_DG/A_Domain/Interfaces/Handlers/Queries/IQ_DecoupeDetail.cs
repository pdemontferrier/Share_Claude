using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeDetail : IQ_Generic<DecoupeDetail>
    {
        // Requête spécifique :
        Task<DecoupeDetail?> HandleGetFirstToCutAsync();
        Task<List<DecoupeDetail>> HandleGetAllForDecoupeBarreIdAsync();
        Task<bool> HandleExistsByDecoupeBarreIdAsync();
        Task<List<DecoupeDetail>> HandleGetToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId);
        Task<List<string>> HandleGetCuttingMachineListToBeSuppliedAsync(int decoupeLotId);
        Task<List<int>> HandleGetArticleInterneIdListToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId);
        Task<List<int>> HandleGetArticleComposeIdListToBeAddedAsync(int decoupeLotId, string decoupeMachineId);
        Task<List<DecoupeDetail>> HandleGetIndice1ByLotAsyncAsync(int decoupeLotId);
        Task<List<DecoupeDetail>> HandleGetIndice2ByLotAsyncAsync(int decoupeLotId);
    }
}