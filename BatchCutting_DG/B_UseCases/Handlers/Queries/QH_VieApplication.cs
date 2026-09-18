using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_VieApplication : QH_Generic<VieApplication>, IQ_VieApplication
    {
        private readonly IR_VieApplication _repositorySpecifique;

        public QH_VieApplication(IR_VieApplication repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Tester si l'application est accéssible
        public async Task<bool> HandleGetAppAccessibilityAsync(int appId)
        {
            return await _repositorySpecifique.IsAppAccessibleAsync(appId);
        }
    }
}
