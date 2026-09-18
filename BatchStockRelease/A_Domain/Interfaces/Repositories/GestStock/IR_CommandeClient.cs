using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_CommandesClient : IR_Generic<CommandeClient>
    {
        Task<List<CommandeClient>> GetByDecoupeLotIdAsync(int decoupeLotId);
    }
}