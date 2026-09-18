using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeBarreWithBarDropToRelease
    {
        Task<List<DTO_DecoupeBarreWithBarDropToRelease>> HandleGetAsync(int decoupeLotId);
    }
}