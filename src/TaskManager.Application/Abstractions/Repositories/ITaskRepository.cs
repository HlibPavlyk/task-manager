using TaskManager.Application.Dtos.Other;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Application.Abstractions.Repositories;

public interface ITaskRepository : IGenericRepository<Task>
{
    Task<PagedResponse<Task>> GetPagedTasksAsync(int page, int size);
}