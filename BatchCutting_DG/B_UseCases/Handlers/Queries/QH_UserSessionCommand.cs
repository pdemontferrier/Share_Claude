using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_UserSessionCommand : QH_Generic<UserSessionCommand>, IQ_UserSessionCommand
    {
        private readonly IR_UserSessionCommand _repositorySpecifique;

        public QH_UserSessionCommand(IR_UserSessionCommand repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Vérifier l'existence de UserSessionCommand par commandType, UserId et AppId
        public async Task<bool> HandleExistsByCommandTypeUserIdAppIdAsync(string commandType, int userId, int appId)
        {
            return await _repositorySpecifique.ExistsByCommandTypeUserIdAppIdAsync(commandType, userId, appId);
        }

        // Requête spécifique : Obtenir les UserSessionCommand par commandType, UserId et AppId
        public async Task<List<UserSessionCommand>> HandleGetBycommandTypeUserIdAppIdAsync(string commandType, int userId, int appId)
        {
            return await _repositorySpecifique.GetByCommandTypeUserIdAppIdAsync(commandType, userId, appId);
        }
    }
}