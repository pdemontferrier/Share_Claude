using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeBarreWithBarNewToRelease
    {
        Task<List<DTO_DecoupeBarreWithBarNewToRelease>> HandleGetAsync(int decoupeLotId);
    }
}