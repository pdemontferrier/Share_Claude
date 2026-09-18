using BatchCutting_DG.A_Domain.GestStock.DTOs;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeDetailWithCut
    {
        Task<DTO_DecoupeDetailWithCut?> GetFirstAsync();
        Task<List<DTO_DecoupeDetailWithCut>> GetAllByLotAndMachineAsync(int decoupeLotId, string machineId);
    }
}