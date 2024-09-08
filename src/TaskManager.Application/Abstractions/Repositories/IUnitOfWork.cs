namespace TaskManager.Application.Abstractions.Repositories;

public interface IUnitOfWork : IDisposable
{
    // Repository for managing User-related operations.
    IUserRepository Users { get; }

    // Repository for managing Task-related operations.
    ITaskRepository Tasks { get; }

    // Commits all changes to the database asynchronously.
    Task SaveChangesAsync();
}