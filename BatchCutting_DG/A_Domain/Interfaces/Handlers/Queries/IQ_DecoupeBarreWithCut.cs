using BatchCutting_DG.A_Domain.GestStock.DTOs;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeBarreWithCut
    {
        Task<DTO_DecoupeBarreWithCut?> HandleGetFirstAsync();
        Task<List<DTO_DecoupeBarreWithCut>> HandleGetAllByLotAndMachineAsync(int decoupeLotId, string machineId);
        Task<List<DTO_DecoupeBarreWithCut>> HandleGetAllDropBarByLotAndMachineAsync(int decoupeLotId, string machineId);

    }
}
