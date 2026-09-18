using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_ChutesMagasin : IQ_Generic<ChutesMagasin>
    {
        // Requête spécifique :
        Task<List<ChutesMagasin>> HandleGetByArticleInterneIdAsync(int articleInterneId);
    }
}