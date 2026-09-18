using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
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