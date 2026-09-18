using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeBarreDetails
    {
        Task<List<DTO_DecoupeBarreDetails>> GetAsync(int decoupeLotId);
    }
}