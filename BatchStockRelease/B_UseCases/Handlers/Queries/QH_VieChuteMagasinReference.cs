using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;


namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_VieChuteMagasinReference : QH_Generic<VieChuteMagasinReference>, IQ_VieChuteMagasinReference
    {
        private readonly IR_VieChuteMagasinReference _repositorySpecifique;

        public QH_VieChuteMagasinReference(IR_VieChuteMagasinReference repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Retourne la liste des chutes pour un article interne donné
        public async Task<List<VieChuteMagasinReference>> HandleGetByArticleInterneIdAsync(int articleInterneId)
        {
            return await _repositorySpecifique.GetByArticleInterneIdAsync(articleInterneId);
        }

    }
}