using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
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