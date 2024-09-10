using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Dtos.Other;
using TaskManager.Application.Dtos.Task;
using TaskManager.Domain.Enums;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : GenericRepository<Task>, ITaskRepository
{
    // Constructor that passes the context to the base GenericRepository.
    public TaskRepository(ApplicationDbContext context) : base(context) { }

    // Retrieves a paginated list of tasks based on the specified page number and size.
    public async Task<PagedResponse<Task>> GetPagedTasksAsync(TaskSortQueryDto taskSortQuery, TaskPageQueryDto taskPageQueryDto, Guid userId)
    {
        var query = Context.Tasks
            .Where(t => t.UserId == userId)
            .AsNoTracking(); // Improves performance by not tracking the entities.

        // Apply Filtering for Status (Enum parsing)
        if (!string.IsNullOrEmpty(taskSortQuery.Status))
        {
            if (Enum.TryParse<Status>(taskSortQuery.Status, true, out var statusEnum)) // Gracefully handle invalid enum values
            {
                query = query.Where(t => t.Status == statusEnum);
            }
            else
            {
                throw new ArgumentException($"Invalid status value: {taskSortQuery.Status}");
            }
        }

        // Apply Filtering for Priority (Enum parsing)
        if (!string.IsNullOrEmpty(taskSortQuery.Priority))
        {
            if (Enum.TryParse<Priority>(taskSortQuery.Priority, true, out var priorityEnum)) // Gracefully handle invalid enum values
            {
                query = query.Where(t => t.Priority == priorityEnum);
            }
            else
            {
                throw new ArgumentException($"Invalid priority value: {taskSortQuery.Priority}");
            }
        }

        // Apply Filtering for DueDate
        if (taskSortQuery.DueDate.HasValue)
        {
            query = query.Where(t => t.DueDate == taskSortQuery.DueDate.Value);
        }

        // Apply Sorting
        if (!string.IsNullOrEmpty(taskSortQuery.SortBy))
        {
            query = taskSortQuery.SortBy switch
            {
                "DueDate" => query.OrderByDescending(t => t.DueDate),
                "Priority" => query.OrderBy(t => t.Priority),
                _ => query.OrderBy(t => t.CreatedAt) // Default sorting
            };
        }

        // Calls a helper method to handle pagination logic.
        return await GetPagedEntitiesAsync(query, taskPageQueryDto.Page, taskPageQueryDto.PageSize);
    }

}
