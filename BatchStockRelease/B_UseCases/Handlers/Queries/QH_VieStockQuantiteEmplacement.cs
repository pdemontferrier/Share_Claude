using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;


namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_VieStockQuantiteEmplacement : QH_Generic<VieStockQuantiteEmplacement>, IQ_VieStockQuantiteEmplacement
    {
        private readonly IR_VieStockQuantiteEmplacement _repositorySpecifique;

        public QH_VieStockQuantiteEmplacement(IR_VieStockQuantiteEmplacement repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Retourne la liste des chutes pour un article interne donné
        public async Task<List<VieStockQuantiteEmplacement>> HandleGetByArticleInterneIdAsync(int articleInterneId)
        {
            return await _repositorySpecifique.GetByArticleInterneIdAsync(articleInterneId);
        }

    }
}