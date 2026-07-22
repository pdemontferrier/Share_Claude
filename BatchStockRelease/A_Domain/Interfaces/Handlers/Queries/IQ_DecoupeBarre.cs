using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeBarre : IQ_Generic<DecoupeBarre>
    {
        // Requête spécifique :
        Task<(int chariotId, string chariotDesignation)> HandleGetChariotInfoAsync(int decoupeLotId);
        Task<List<DecoupeBarre>> HandleGetAllForArticleInterneIdAsync(int decoupeLotId, int articleInterneId);
        Task<List<int>> HandleGetDistinctArticleInterneIdAsync(int decoupeLotId);
        Task<List<DecoupeBarre>> HandleGetAllBarNewByIdStock(int decoupeLotId, int idStock);

        Task<List<DecoupeBarre>> HandleGetAllocatedAsync(int decoupeLotId);
        Task<bool> HandleCheckAllocated(int decoupeLotId);

        Task<List<DecoupeBarre>> HandleGetBarNewNotAllocatedAsync(int decoupeLotId);
        Task<bool> HandleCheckBarNewNotAllocated(int decoupeLotId);

        Task<List<DecoupeBarre>> HandleGetBarDropToReleaseAsync(int decoupeLotId);
        Task<bool> HandleCheckBarDropToReleaseAsync(int decoupeLotId);
        Task<List<DecoupeBarre>> HandleGetBarNewToReleaseAsync(int decoupeLotId);
        Task<bool> HandleCheckBarNewToReleaseAsync(int decoupeLotId);
        Task<List<DecoupeBarre>> HandleCheckBarNewAllocatedToReallocateAsync(int decoupeLotId);


        Task<List<DecoupeBarre>> HandleGetBarNewOutOfStockAsync(int decoupeLotId);
        Task<bool> HandleCheckBarNewOutOfStock(int decoupeLotId);
    }
}