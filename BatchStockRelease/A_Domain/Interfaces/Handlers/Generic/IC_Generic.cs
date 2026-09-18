using System.Runtime.CompilerServices;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Generic
{
    public interface IC_Generic<T>
    {
        // Commande générique
        Task HandleAddAsync(T entity, string callChain, string? handlerCommand = null, string? commandMethod = null);
        Task HandleUpdateAsync(T entity, string callChain, string? handlerCommand = null, string? commandMethod = null);
        Task HandleUpdateRangeAsync(IEnumerable<T> entities, string callChain, string? handlerCommand = null, string? commandMethod = null);
        Task HandleDeleteAsync(int id, string callChain, string? handlerCommand = null, string? commandMethod = null);
        Task HandleSaveChangesAsync();
    }
}