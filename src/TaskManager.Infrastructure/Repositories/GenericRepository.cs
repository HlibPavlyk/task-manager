using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Dtos.Other;

namespace TaskManager.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    // The database context to interact with the database.
    protected readonly ApplicationDbContext Context;

    // Constructor that sets the context for the repository.
    protected GenericRepository(ApplicationDbContext context)
    {
        Context = context;
    }

    // Fetches an entity by its unique ID.
    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await Context.Set<TEntity>()
            .FindAsync(id);
    }

    // Checks if any entities match the specified condition.
    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return Context.Set<TEntity>()
            .AnyAsync(predicate);
    }

    // Fetches all entities without tracking changes (useful for read-only scenarios).
    public async Task<IEnumerable<TEntity>?> GetAllAsync()
    {
        return await Context.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync();
    }

    // Adds a new entity to the context asynchronously.
    public async Task AddAsync(TEntity entity)
    {
        await Context.Set<TEntity>()
            .AddAsync(entity);
    }

    // Updates an existing entity in the context.
    public void Update(TEntity entity)
    {
        Context.Set<TEntity>()
            .Update(entity);
    }

    // Removes an entity from the context.
    public void Remove(TEntity entity)
    {
        Context.Set<TEntity>()
            .Remove(entity);
    }

    // Helper method to handle paginated queries for entities.
    protected async Task<PagedResponse<T>> GetPagedEntitiesAsync<T>(IQueryable<T> query, int page, int size) where T : class
    {
        var totalItems = await query.CountAsync();
        if (totalItems == 0)
        {
            return new PagedResponse<T>
            {
                Items = new List<T>(),
                TotalPages = 0
            };
        }

        // Fetch the paginated items based on the page number and size.
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        // Calculate the total number of pages.
        var totalPages = (int)Math.Ceiling(totalItems / (double)size);

        return new PagedResponse<T>
        {
            Items = items,
            TotalPages = totalPages
        };
    }
}
