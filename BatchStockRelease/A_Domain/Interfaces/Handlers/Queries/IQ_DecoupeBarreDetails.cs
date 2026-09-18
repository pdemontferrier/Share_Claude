using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeBarreDetails
    {
        Task<List<DTO_DecoupeBarreDetails>> HandleGetAsync(int decoupeLotId);
    }
}