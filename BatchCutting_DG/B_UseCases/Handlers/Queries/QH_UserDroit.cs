using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_UserDroit : QH_Generic<UserDroit>, IQ_UserDroit
    {
        private readonly IR_UserDroit _repositorySpecifique;

        public QH_UserDroit(IR_UserDroit repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Vérifier si une action utilisateur est déclarer pour un utilisateur donné.
        public async Task<bool> HandleGetUserActionAsync(int userId, int actionId)
        {
            return await _repositorySpecifique.HasUserActionAsync(userId, actionId);
        }
    }
}