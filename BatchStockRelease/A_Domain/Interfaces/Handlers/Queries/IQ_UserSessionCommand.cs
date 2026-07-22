using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_UserSessionCommand : IQ_Generic<UserSessionCommand>
    {
        // Requête spécifique :
        Task<bool> HandleExistsByCommandTypeUserIdAppIdAsync(string commandType, int userId, int appId);
        Task<List<UserSessionCommand>> HandleGetBycommandTypeUserIdAppIdAsync(string commandType, int userId, int appId);
    }
}
