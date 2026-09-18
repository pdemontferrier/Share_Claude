using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic;
using System.Runtime.CompilerServices;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_DecoupeDetail : IC_Generic<DecoupeDetail>
    {
        Task HandleDisableOptimisationForBarreAsync(int decoupeBarreId, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
    }
}