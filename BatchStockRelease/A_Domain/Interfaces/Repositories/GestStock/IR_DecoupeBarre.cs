using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeBarre : IR_Generic<DecoupeBarre>
    {
        Task<(int chariotId, string chariotDesignation)> GetChariotInfoAsync(int decoupeLotId);
        Task<List<DecoupeBarre>> GetAllForArticleInterneIdAsync(int decoupeLotId, int articleInterneId);
        Task<List<int>> GetDistinctArticleInterneIdAsync(int decoupeLotId);
        Task<List<DecoupeBarre>> GetAllBarNewByIdStock(int decoupeLotId, int idStock);

        Task<List<DecoupeBarre>> GetAllocatedAsync(int decoupeLotId);
        Task<bool> CheckAllocated(int decoupeLotId);

        Task<List<DecoupeBarre>> GetBarNewNotAllocatedAsync(int decoupeLotId);
        Task<bool> CheckBarNewNotAllocated(int decoupeLotId);

        Task<List<DecoupeBarre>> GetBarDropToReleaseAsync(int decoupeLotId);
        Task<bool> CheckBarDropToReleaseAsync(int decoupeLotId);
        Task<List<DecoupeBarre>> GetBarNewToReleaseAsync(int decoupeLotId);
        Task<bool> CheckBarNewToReleaseAsync(int decoupeLotId);
        Task<List<DecoupeBarre>> GetBarNewAllocatedToReallocateAsync(int decoupeLotId);

        Task<List<DecoupeBarre>> GetBarNewOutOfStockAsync(int decoupeLotId);
        Task<bool> CheckBarNewOutOfStock(int decoupeLotId);
    }
}