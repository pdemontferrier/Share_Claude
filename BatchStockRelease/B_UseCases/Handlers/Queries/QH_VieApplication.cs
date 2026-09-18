using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
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
        public async Task<bool> HandleGetApplicationAccessibilityAsync(int appId)
        {
            return await _repositorySpecifique.IsAppAccessibleAsync(appId);
        }
    }
}
