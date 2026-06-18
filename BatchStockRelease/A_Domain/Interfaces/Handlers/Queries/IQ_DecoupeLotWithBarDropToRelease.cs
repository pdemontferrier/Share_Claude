using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeLotWithBarDropToRelease
    {
        // Requête spécifique :
        Task<List<DTO_DecoupeLotWithBarDropToRelease>> HandleGetLotsToReleaseAsync();
    }
}
