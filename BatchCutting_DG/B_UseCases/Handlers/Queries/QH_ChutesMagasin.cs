using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;


namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_ChutesMagasin : QH_Generic<ChutesMagasin>, IQ_ChutesMagasin
    {
        private readonly IR_ChutesMagasin _repositorySpecifique;

        public QH_ChutesMagasin(IR_ChutesMagasin repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Retourne la liste des chutes pour un article interne donné
        public async Task<List<ChutesMagasin>> HandleGetByArticleInterneIdAsync(int articleInterneId)
        {
            return await _repositorySpecifique.GetByArticleInterneIdAsync(articleInterneId);
        }

    }
}