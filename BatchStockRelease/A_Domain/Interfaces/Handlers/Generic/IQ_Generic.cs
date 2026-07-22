namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Generic
{
    public interface IQ_Generic<T>
    {
        Task<T?> HandleGetByIdAsync(int id);
        Task<T?> HandleGetFirstOrDefaultAsync();
        Task<bool> HandleGetAnyAsync(int id);
        Task<List<T>> HandleGetAllAsync();
        Task<List<T>> HandleGetAllAsNoTrackingAsync();
    }
}