using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_UserSessionCommand : IC_Generic<UserSessionCommand>
    {
        Task HandleAddCloseSessionCommandAsync(int targetSessionId, string callChain);
        Task HandleDeleteCloseCommandForSessionAsync(string callChain);
    }
}
