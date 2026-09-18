using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_UserSession : IC_Generic<UserSession>
    {
        Task HandleCreateNewUserSessionAsync(string callChain);
        Task HandleUpdateUserSessionAsync(UserSession entity, bool isConnected, string callChain);
        Task HandleDeleteAdditionalSessions(IEnumerable<UserSession> additionalSessions, string callChain);
    }
}
