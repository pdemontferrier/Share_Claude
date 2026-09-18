using BatchCutting_DG.A_Domain.GestStock.DTOs;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeLotWithCut
    {
        Task<List<DTO_DecoupeLotWithCut>> GetDecoupeLotWithCutAsync();
    }
}