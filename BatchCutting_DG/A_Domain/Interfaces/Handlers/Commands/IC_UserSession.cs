using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;
using System.Runtime.CompilerServices;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_UserSession : IC_Generic<UserSession>
    {
        Task HandleCreateNewUserSessionAsync(string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
        Task HandleUpdateUserSessionAsync(UserSession entity, bool isConnected, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
        Task HandleDeleteAdditionalSessions(IEnumerable<UserSession> additionalSessions, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
    }
}
