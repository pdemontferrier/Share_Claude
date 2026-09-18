using BatchCutting_DG.A_Domain.GestStock.DTOs;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeLotWithCut
    {
        Task<List<DTO_DecoupeLotWithCut>> HandleAsync();
    }
}
