using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;
using System.Runtime.CompilerServices;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_UserAppMessage : IC_Generic<UserAppMessage>
    {
        Task HandleMarkAsReadAsync(int messageId, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
    }
}
