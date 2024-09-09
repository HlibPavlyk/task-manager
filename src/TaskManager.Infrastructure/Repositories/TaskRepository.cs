using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Dtos.Other;
using TaskManager.Application.Dtos.Task;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : GenericRepository<Task>, ITaskRepository
{
    // Constructor that passes the context to the base GenericRepository.
    public TaskRepository(ApplicationDbContext context) : base(context) { }

    // Retrieves a paginated list of tasks based on the specified page number and size.
    public async Task<PagedResponse<Task>> GetPagedTasksAsync(TaskQueryDto taskQuery, Guid userId)
    {
        var query = Context.Tasks
            .Where(t => t.UserId == userId)
            .AsNoTracking(); // Improves performance by not tracking the entities.
        
        // Apply Filtering
        if (!string.IsNullOrEmpty(taskQuery.Status))
            query = query.Where(t => t.Status.ToString() == taskQuery.Status); // Assuming status is an enum
        if (taskQuery.DueDate.HasValue)
            query = query.Where(t => t.DueDate == taskQuery.DueDate.Value);
        if (!string.IsNullOrEmpty(taskQuery.Priority))
            query = query.Where(t => t.Priority.ToString() == taskQuery.Priority); // Assuming priority is an enum

        // Apply Sorting
        if (!string.IsNullOrEmpty(taskQuery.SortBy))
        {
            query = taskQuery.SortBy switch
            {
                "DueDate" => query.OrderBy(t => t.DueDate),
                "Priority" => query.OrderBy(t => t.Priority),
                _ => query.OrderBy(t => t.CreatedAt) // Default sorting
            };
        }

        // Calls a helper method to handle pagination logic.
        return await GetPagedEntitiesAsync(query, taskQuery.Page, taskQuery.PageSize);
    }
}
