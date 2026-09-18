using BatchCutting_DG.A_Domain.GestStock.DTOs;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeDetailWithCut
    {
        Task<DTO_DecoupeDetailWithCut?> HandleGetFirstAsync();
        Task<List<DTO_DecoupeDetailWithCut>> HandleGetAllByLotAndMachineAsync(int decoupeLotId, string machineId);
    }
}
