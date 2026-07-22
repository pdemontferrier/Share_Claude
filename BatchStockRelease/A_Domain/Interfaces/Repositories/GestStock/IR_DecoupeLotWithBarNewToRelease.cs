using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeLotWithBarNewToRelease
    {
        Task<List<DTO_DecoupeLotWithBarNewToRelease>> GetLotsToReleaseAsync();
        Task<List<DTO_DecoupeLotWithBarNewToRelease>> GetLotsOutOfStockAsync();
    }
}
