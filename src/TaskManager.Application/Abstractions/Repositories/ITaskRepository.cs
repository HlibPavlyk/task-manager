using TaskManager.Application.Dtos.Other;
using TaskManager.Application.Dtos.Task;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Application.Abstractions.Repositories;

public interface ITaskRepository : IGenericRepository<Task>
{
    // Retrieves a paginated list of tasks based on the specified page number and size.
    Task<PagedResponse<Task>> GetPagedTasksAsync(TaskQueryDto taskQuery, Guid userId);
}