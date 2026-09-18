using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_UserSession : QH_Generic<UserSession>, IQ_UserSession
    {
        private readonly IR_UserSession _repositorySpecifique;

        public QH_UserSession(IR_UserSession repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Obtenir les UserSession par appId et userId
        public async Task<List<UserSession>> HandleGetByUserIdAppIdAsync(int userId, int appId)
        {
            return await _repositorySpecifique.GetByUserIdAppIdAsync(userId, appId);
        }


        // Requête spécifique : Obtenir les SessionId par appId et userId
        public async Task<int> HandleGetSessionIdAsync(int userId, int appId)
        {
            // Récupérer la session de l'utilisateur
            var existingSessions = await HandleGetByUserIdAppIdAsync(userId, appId);

            if (existingSessions.Any())
            {
                // Met à jour la session existante
                var existingSession = existingSessions.First();
                return existingSession.Id;
            }
            else
            {
                return 0;
            }
        }
    }
}