using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeLotWithBarDropToRelease
    {
        Task<List<DTO_DecoupeLotWithBarDropToRelease>> GetLotsToReleaseAsync();
    }
}