using TaskManager.Application.Dtos.Other;
using TaskManager.Application.Dtos.Task;

namespace TaskManager.Application.Abstractions.Services;

public interface ITaskService
{
    // Creates a new task and returns its unique identifier.
    Task<Guid> CreateTaskAsync(TaskPostDto taskPostDto);

    // Retrieves a paginated list of tasks based on filtering and sorting options.
    Task<PagedResponse<TaskGetDto>> GetPagedTasksAsync(TaskQueryDto taskQueryDto);

    // Retrieves task details by its unique identifier.
    Task<TaskGetDto> GetTaskByIdAsync(Guid id);

    // Updates an existing task and returns the task's unique identifier.
    Task<Guid> UpdateTaskAsync(Guid id, TaskPostDto taskPutDto);

    // Deletes a task by its unique identifier.
    Task DeleteTaskAsync(Guid id);
}