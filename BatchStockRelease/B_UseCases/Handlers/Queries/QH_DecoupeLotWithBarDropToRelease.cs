using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeLotWithBarDropToRelease : IQ_DecoupeLotWithBarDropToRelease
    {
        private readonly IR_DecoupeLotWithBarDropToRelease _repository;

        public QH_DecoupeLotWithBarDropToRelease(IR_DecoupeLotWithBarDropToRelease repository)
        {
            _repository = repository;
        }

        public async Task<List<DTO_DecoupeLotWithBarDropToRelease>> HandleGetLotsToReleaseAsync()
        {
            return await _repository.GetLotsToReleaseAsync();
        }
    }
}