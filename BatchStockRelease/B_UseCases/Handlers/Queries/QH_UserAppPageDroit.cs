using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_UserAppPageDroit : QH_Generic<UserAppPageDroit>, IQ_UserAppPageDroit
    {
        private readonly IR_UserAppPageDroit _repositorySpecifique;

        public QH_UserAppPageDroit(IR_UserAppPageDroit repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Obtenir le premier UserAppPageAccess en fonction de userId, appId
        public async Task<List<UserAppPageDroit>> HandleGetByUserIdAppIdAsync(int userId, int appId)
        {
            return await _repositorySpecifique.GetByUserIdAppIdAsync(userId, appId);
        }
    }
}