using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeLotWithBarNewToRelease
    {
        // Requête spécifique :
        Task<List<DTO_DecoupeLotWithBarNewToRelease>> HandleGetLotsToReleaseAsync();
        Task<List<DTO_DecoupeLotWithBarNewToRelease>> HandleGetLotsOutOfStockAsync();
    }
}
