using System.Runtime.CompilerServices;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Generic
{
    public interface IC_Generic<T>
    {
        // Commande générique
        Task HandleAddAsync(T entity, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
        Task HandleUpdateAsync(T entity, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
        Task HandleUpdateRangeAsync(IEnumerable<T> entities, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
        Task HandleDeleteAsync(int id, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "");
        Task HandleSaveChangesAsync();
        string BuildAppServiceCaller(string explicitCaller, string actualCaller);
    }
}