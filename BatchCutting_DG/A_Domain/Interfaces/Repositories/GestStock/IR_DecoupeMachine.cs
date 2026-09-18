using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_DecoupeMachine : IR_Generic<DecoupeMachine>
    {
        Task<DecoupeMachine?> GetByDeviceIpAddressAsync(string ipAddress);
    }
}