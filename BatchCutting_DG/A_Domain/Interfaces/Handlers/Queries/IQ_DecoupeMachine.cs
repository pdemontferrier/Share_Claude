using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeMachine : IQ_Generic<DecoupeMachine>
    {
        // Requête spécifique :
        Task<DecoupeMachine?> HandleGetByDeviceIpAddressAsync(string ipDeviceAddress);
    }
}