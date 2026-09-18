using BatchCutting_DG.A_Domain.GestStock.DTOs;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeBarreWithCut
    {
        Task<DTO_DecoupeBarreWithCut?> GetFirstAsync();
        Task<List<DTO_DecoupeBarreWithCut>> GetAllByLotAndMachineAsync(int decoupeLotId, string machineId);
        Task<List<DTO_DecoupeBarreWithCut>> GetAllDropBarByLotAndMachineAsync(int decoupeLotId, string machineId);
    }
}