using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;
using System.Runtime.CompilerServices;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_UserSessionCommand : IC_Generic<UserSessionCommand>
    {
        Task HandleAddCloseSessionCommandAsync(int targetSessionId, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
        Task HandleDeleteCloseCommandForSessionAsync(string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
    }
}
