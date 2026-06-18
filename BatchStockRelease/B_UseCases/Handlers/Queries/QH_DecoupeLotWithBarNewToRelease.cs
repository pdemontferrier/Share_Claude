using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeLotWithBarNewToRelease : IQ_DecoupeLotWithBarNewToRelease
    {
        private readonly IR_DecoupeLotWithBarNewToRelease _repository;

        public QH_DecoupeLotWithBarNewToRelease(IR_DecoupeLotWithBarNewToRelease repository)
        {
            _repository = repository;
        }

        public async Task<List<DTO_DecoupeLotWithBarNewToRelease>> HandleGetLotsToReleaseAsync()
        {
            return await _repository.GetLotsToReleaseAsync();
        }

        public async Task<List<DTO_DecoupeLotWithBarNewToRelease>> HandleGetLotsOutOfStockAsync()
        {
            return await _repository.GetLotsOutOfStockAsync();
        }
    }
}