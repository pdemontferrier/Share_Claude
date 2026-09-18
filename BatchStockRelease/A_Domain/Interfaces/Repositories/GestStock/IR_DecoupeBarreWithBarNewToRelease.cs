using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeBarreWithBarNewToRelease
    {
        Task<List<DTO_DecoupeBarreWithBarNewToRelease>> GetAsync(int decoupeLotId);
    }
}