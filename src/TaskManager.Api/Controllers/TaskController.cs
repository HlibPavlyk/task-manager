using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Responses;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Dtos.Task;
using TaskManager.Application.Services;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TaskController : Controller
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskService> _logger;

    // Constructor injects the task service and logger.
    public TaskController(ITaskService taskService, ILogger<TaskService> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }
    
    // Creates a new task for the authenticated user.
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskPostDto taskPostDto)
    {
        try
        {
            var id = await _taskService.CreateTaskAsync(taskPostDto);
            return CreatedAtAction(nameof(GetTaskById), new { id }, await _taskService.GetTaskByIdAsync(id));
        }
        catch (InvalidDataException e)
        {
            return LoggingHelper.LogAndReturnBadRequest(_logger, e);
        }
        catch (UnauthorizedAccessException e)
        {
            return LoggingHelper.LogAndReturnBadRequest(_logger, e);
        }
        catch (NotFoundException e)
        {
            return LoggingHelper.LogAndReturnNotFound(_logger, e);
        }
    }
    
    // Retrieves a paginated list of tasks for the authenticated user based on filters.
    [HttpGet]
    public async Task<IActionResult> GetPagedTasks([FromQuery] TaskSortQueryDto taskSortQueryDto, [FromQuery] int page = 1, [FromQuery] int size = 10)
    {
        try
        {
            var tasks = await _taskService.GetPagedTasksAsync(taskSortQueryDto, new TaskPageQueryDto(page, size));
            return Ok(tasks);
        }
        catch (ArgumentException e)
        {
            return LoggingHelper.LogAndReturnBadRequest(_logger, e);
        }
        catch (UnauthorizedAccessException e)
        {
            return LoggingHelper.LogAndReturnBadRequest(_logger, e);
        }
        catch (NotFoundException e)
        {
            return LoggingHelper.LogAndReturnNotFound(_logger, e);
        }
    }
    
    // Retrieves a specific task by its ID.
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById([FromRoute] Guid id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            return Ok(task);
        }
        catch (UnauthorizedAccessException e)
        {
            return LoggingHelper.LogAndReturnBadRequest(_logger, e);
        }
        catch (NotFoundException e)
        {
            return LoggingHelper.LogAndReturnNotFound(_logger, e);
        }
    }
    
    // Updates an existing task by its ID.
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] TaskPostDto taskPutDto)
    {
        try
        {
            var updatedId = await _taskService.UpdateTaskAsync(id, taskPutDto);
            return Ok(await _taskService.GetTaskByIdAsync(updatedId));
        }
        catch (UnauthorizedAccessException e)
        {
            return LoggingHelper.LogAndReturnBadRequest(_logger, e);
        }
        catch (NotFoundException e)
        {
            return LoggingHelper.LogAndReturnNotFound(_logger, e);
        }
    }
    
    // Deletes a task by its ID.
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask([FromRoute] Guid id)
    {
        try
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent(); // Returns NoContent (204) when successfully deleted.
        }
        catch (UnauthorizedAccessException e)
        {
            return LoggingHelper.LogAndReturnBadRequest(_logger, e);
        }
        catch (NotFoundException e)
        {
            return LoggingHelper.LogAndReturnNotFound(_logger, e);
        }
    }
}
