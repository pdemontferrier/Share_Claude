using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;

namespace BatchStockRelease.C_Infrastructure.Repositories.Generic
{
    public class CR_Generic<T> : IR_Generic<T> where T : class
    {
        protected readonly IDbContextFactory<GestStockContext> _contextFactory;

        public CR_Generic(IDbContextFactory<GestStockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        // Chaque méthode est isolée, thread-safe, et ne conserve aucun état global


        // CRUDS
        // Create
        public async Task AddAsync(T entity)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
        }


        // Read
        public async Task<T?> GetByIdAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetFirstOrDefaultAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<T>().FirstOrDefaultAsync();
        }

        public async Task<bool> GetAnyAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var entity = await context.Set<T>().FindAsync(id);
            return entity != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<T>().ToListAsync();
        }

        public async Task<List<T>> GetAllAsNoTrackingAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<T>().AsNoTracking().ToListAsync();
        }

        public IQueryable<T> GetAllQueryable()
        {
            // ⚠️ À utiliser avec prudence : chaque appel DOIT consommer immédiatement la requête.
            var context = _contextFactory.CreateDbContext();
            return context.Set<T>().AsNoTracking();
        }

        public async Task<List<T>> GetFilteredAsync(Expression<Func<T, bool>> predicate)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Set<T>().Where(predicate).ToListAsync();
        }


        // Update
        public async Task UpdateAsync(T entity)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<T>().UpdateRange(entities);
            await context.SaveChangesAsync();
        }


        // Delete
        public async Task DeleteAsync(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var entity = await context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
            }
        }


        // Save changes
        public async Task SaveChangesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            await context.SaveChangesAsync();
        }
    }
}