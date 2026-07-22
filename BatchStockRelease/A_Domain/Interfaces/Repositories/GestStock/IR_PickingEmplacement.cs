using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;


namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_PickingEmplacement : IR_Generic<PickingEmplacement>
    {
        Task<List<PickingEmplacement>> GetChariotListAsync();
    }
}
