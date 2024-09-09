using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Dtos.Other;
using TaskManager.Application.Dtos.Task;
using TaskManager.Domain.Exceptions;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TaskService> _logger;

    // Constructor that injects dependencies.
    public TaskService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper, ILogger<TaskService> logger)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _logger = logger;
    }

    // Creates a new task for the authenticated user.
    public async Task<Guid> CreateTaskAsync(TaskPostDto taskPostDto)
    {
        var userId = GetAuthenticatedUserId(); // Get the ID of the authenticated user.
        _logger.LogInformation("Creating task for user {UserId}", userId);

        // Check if the user exists in the system.
        if (!await _unitOfWork.Users.AnyAsync(x => x.Id == userId))
            throw new NotFoundException($"User with ID {userId} not found.");

        // Map the DTO to the Task entity and set the user ID.
        var task = _mapper.Map<Task>(taskPostDto);
        task.UserId = userId;

        await _unitOfWork.Tasks.AddAsync(task); // Add the task to the database.
        await _unitOfWork.SaveChangesAsync(); // Save the changes.

        _logger.LogInformation("Task {TaskId} created for user {UserId}", task.Id, userId);
        return task.Id;
    }

    // Retrieves a paginated list of tasks for the authenticated user.
    public async Task<PagedResponse<TaskGetDto>> GetPagedTasksAsync(TaskQueryDto taskQueryDto)
    {
        var userId = GetAuthenticatedUserId(); // Get the ID of the authenticated user.
        _logger.LogInformation("Retrieving tasks for user {UserId}", userId);

        var tasks = await _unitOfWork.Tasks.GetPagedTasksAsync(taskQueryDto, userId);
        if (tasks.TotalPages == 0)
            throw new NotFoundException($"No tasks found for user with ID {userId}.");

        _logger.LogInformation("{TaskCount} tasks found for user {UserId}", tasks.Items.Count(), userId);
        return _mapper.Map<PagedResponse<TaskGetDto>>(tasks);
    }

    // Retrieves a task by ID for the authenticated user.
    public async Task<TaskGetDto> GetTaskByIdAsync(Guid id)
    {
        var userId = GetAuthenticatedUserId(); // Get the ID of the authenticated user.
        _logger.LogInformation("Retrieving task {TaskId} for user {UserId}", id, userId);

        var task = await _unitOfWork.Tasks.GetByIdAsync(id); // Fetch the task by ID.
        if (task == null)
            throw new NotFoundException($"Task with ID {id} not found.");

        if (task.UserId != userId)
            throw new UnauthorizedAccessException($"User with ID {userId} does not have permission to access task with ID {id}.");

        return _mapper.Map<TaskGetDto>(task); // Map and return the task.
    }

    // Updates a task for the authenticated user.
    public async Task<Guid> UpdateTaskAsync(Guid id, TaskPostDto taskPutDto)
    {
        var userId = GetAuthenticatedUserId(); // Get the ID of the authenticated user.
        _logger.LogInformation("Updating task {TaskId} for user {UserId}", id, userId);

        var task = await _unitOfWork.Tasks.GetByIdAsync(id); // Fetch the task by ID.
        if (task == null)
            throw new NotFoundException($"Task with ID {id} not found.");

        if (task.UserId != userId)
            throw new UnauthorizedAccessException($"User with ID {userId} does not have permission to update task with ID {id}.");

        _mapper.Map(taskPutDto, task); // Map the updated data to the task entity.
        _unitOfWork.Tasks.Update(task); // Update the task in the database.
        await _unitOfWork.SaveChangesAsync(); // Save the changes.

        _logger.LogInformation("Task {TaskId} updated for user {UserId}", task.Id, userId);
        return task.Id;
    }

    // Deletes a task for the authenticated user.
    public async System.Threading.Tasks.Task DeleteTaskAsync(Guid id)
    {
        var userId = GetAuthenticatedUserId(); // Get the ID of the authenticated user.
        _logger.LogInformation("Deleting task {TaskId} for user {UserId}", id, userId);

        var task = await _unitOfWork.Tasks.GetByIdAsync(id); // Fetch the task by ID.
        if (task == null)
            throw new NotFoundException($"Task with ID {id} not found.");

        if (task.UserId != userId)
            throw new UnauthorizedAccessException($"User with ID {userId} does not have permission to delete task with ID {id}.");

        _unitOfWork.Tasks.Remove(task); // Remove the task from the database.
        await _unitOfWork.SaveChangesAsync(); // Save the changes.

        _logger.LogInformation("Task {TaskId} deleted for user {UserId}", id, userId);
    }

    // Helper method to retrieve the authenticated user's ID from the HTTP context.
    private Guid GetAuthenticatedUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user is not { Identity.IsAuthenticated: true } || !Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid.");

        _logger.LogInformation("Authenticated user {UserId}", userId); // Log the user ID.
        return userId;
    }
}
