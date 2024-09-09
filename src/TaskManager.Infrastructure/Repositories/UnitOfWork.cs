using TaskManager.Application.Abstractions.Repositories;

namespace TaskManager.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private bool _disposed;

    // Exposes the User and Task repositories through the Unit of Work.
    public IUserRepository Users { get; }
    public ITaskRepository Tasks { get; }

    // Constructor initializes the repositories with the same database context.
    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        Users = new UserRepository(_dbContext);
        Tasks = new TaskRepository(_dbContext);
    }

    // Saves all changes to the database asynchronously.
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    // Disposes the database context to free resources when no longer needed.
    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _dbContext.Dispose(); // Dispose the database context.
        }
        _disposed = true;
    }

    // Public Dispose method to be called when the object is no longer in use.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Prevents the garbage collector from calling the finalizer.
    }
}
