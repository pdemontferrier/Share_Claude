using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_DecoupeBarre : IC_Generic<DecoupeBarre>
    {
        // Commande spécifique : Mettre à jour un ensemble d'enregistrement de DecoupeBarre pour annuler l'allocation
        Task HandleDisableAllocationForBarreAsync(IEnumerable<DecoupeBarre> itemsToReset, string callChain);
    }
}