using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;


namespace BatchCutting_DG.B_UseCases.Handlers.Queries
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