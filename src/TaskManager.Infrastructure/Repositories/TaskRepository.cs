using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Dtos.Other;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : GenericRepository<Task>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context) { }

    public async Task<PagedResponse<Task>> GetPagedTasksAsync(int page, int size)
    {
        var query = Context.Tasks
            .AsNoTracking();

        return await GetPagedEntitiesAsync(query, page, size);
    }
}