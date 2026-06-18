using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_UserApplication : QH_Generic<UserApplication>, IQ_UserApplication
    {
        private readonly IR_UserApplication _repositorySpecifique;

        public QH_UserApplication(IR_UserApplication repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Vérifier si une action utilisateur est déclarer pour un utilisateur donné.
        public async Task<bool> HandleHasUserAccessAppAsync(int userId, int appId)
        {
            return await _repositorySpecifique.HasUserAccessAppAsync(userId, appId);
        }
    }
}