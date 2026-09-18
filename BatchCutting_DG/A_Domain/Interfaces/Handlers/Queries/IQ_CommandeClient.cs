using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_CommandeClient : IQ_Generic<CommandeClient>
    {
        // Requête spécifique :
        Task<List<CommandeClient>> HandleGetByIdDecoupeLotAsync(int decoupeLotId);
    }
}
