using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_User : IQ_Generic<User>
    {
        // Requête spécifique :
        Task<User?> HandleGetSingleAsync(string loginId, string password);
        Task<string> HandleGetUserFullNameAsync(int userId);
        Task<User?> HandleGetByLoginWindowsAsync(string loginWindows);
    }
}
