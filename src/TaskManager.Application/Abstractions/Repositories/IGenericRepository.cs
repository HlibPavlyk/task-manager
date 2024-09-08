using System.Linq.Expressions;

namespace TaskManager.Application.Abstractions.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    // Checks if any entities match the given condition.
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    // Retrieves an entity by its unique identifier (ID), or null if not found.
    Task<TEntity?> GetByIdAsync(Guid id);

    // Retrieves all entities of the specified type.
    Task<IEnumerable<TEntity>?> GetAllAsync();

    // Adds a new entity to the database asynchronously.
    Task AddAsync(TEntity entity);

    // Updates an existing entity in the database.
    void Update(TEntity entity);

    // Removes an entity from the database.
    void Remove(TEntity entity);
}
