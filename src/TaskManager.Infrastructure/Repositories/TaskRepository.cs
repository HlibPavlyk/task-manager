using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Dtos.Other;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : GenericRepository<Task>, ITaskRepository
{
    // Constructor that passes the context to the base GenericRepository.
    public TaskRepository(ApplicationDbContext context) : base(context) { }

    // Retrieves a paginated list of tasks based on the specified page number and size.
    public async Task<PagedResponse<Task>> GetPagedTasksAsync(int page, int size)
    {
        var query = Context.Tasks
            .AsNoTracking(); // Improves performance by not tracking the entities.

        // Calls a helper method to handle pagination logic.
        return await GetPagedEntitiesAsync(query, page, size);
    }
}
