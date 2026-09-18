using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_DecoupeBarre : IQ_Generic<DecoupeBarre>
    {
        // Requête spécifique :
        Task<(int chariotId, string chariotDesignation)> HandleGetChariotInfoForLotAsync(int decoupeLotId);
    }
}