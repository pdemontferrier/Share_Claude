using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeLotWithCut : IQ_DecoupeLotWithCut
    {
        private readonly IR_DecoupeLotWithCut _repository;

        public QH_DecoupeLotWithCut(IR_DecoupeLotWithCut repository)
        {
            _repository = repository;
        }

        public async Task<List<DTO_DecoupeLotWithCut>> HandleAsync()
        {
            return await _repository.GetDecoupeLotWithCutAsync();
        }
    }
}