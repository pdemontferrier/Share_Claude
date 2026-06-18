namespace BatchStockRelease.A_Domain.Interfaces.Repositories.Generic
{
    public interface IR_Generic<T>
    {
        // CRUDS
        // Create
        Task AddAsync(T entity);

        // Read
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetFirstOrDefaultAsync();
        Task<bool> GetAnyAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsNoTrackingAsync();
        IQueryable<T> GetAllQueryable();

        // Update
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);

        // Delete
        Task DeleteAsync(int id);

        // Save
        Task SaveChangesAsync();
    }
}