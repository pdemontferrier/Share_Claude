using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeBarreWithBarDropToRelease
    {
        Task<List<DTO_DecoupeBarreWithBarDropToRelease>> GetAsync(int decoupeLotId);
    }
}