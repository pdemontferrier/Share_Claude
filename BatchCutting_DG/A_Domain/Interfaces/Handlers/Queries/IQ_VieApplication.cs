using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_VieApplication : IQ_Generic<VieApplication>
    {
        // Requête spécifique :
        Task<bool> HandleGetAppAccessibilityAsync(int appId);
    }
}
