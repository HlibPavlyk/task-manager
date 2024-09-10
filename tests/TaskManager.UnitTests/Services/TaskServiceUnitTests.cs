using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Abstractions.Repositories;
using TaskManager.Application.Dtos.Other;
using TaskManager.Application.Dtos.Task;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.UnitTests.Services;

public class TaskServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        Mock<ILogger<TaskService>> loggerMock = new();
        _taskService = new TaskService(_unitOfWorkMock.Object, _httpContextAccessorMock.Object, _mapperMock.Object, loggerMock.Object);
    }

    private void SetupHttpContextAccessor(Guid userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(claimsPrincipal);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_ShouldCreateTask_WhenValidDataProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var taskPostDto = new TaskPostDto ("Test Task", null, null, Status.Completed, Priority.High); 
        var task = new Task { Id = Guid.NewGuid(), Title = taskPostDto.Title, UserId = userId };

        _unitOfWorkMock.Setup(x => x.Users.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);
        _mapperMock.Setup(x => x.Map<Task>(taskPostDto)).Returns(task);
        _unitOfWorkMock.Setup(x => x.Tasks.AddAsync(task)).Returns(System.Threading.Tasks.Task.CompletedTask);


        // Act
        var result = await _taskService.CreateTaskAsync(taskPostDto);

        // Assert
        Assert.Equal(task.Id, result);
        _unitOfWorkMock.Verify(x => x.Tasks.AddAsync(task), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async  System.Threading.Tasks.Task CreateTaskAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var taskPostDto = new TaskPostDto ("Test Task", null, null, Status.Completed, Priority.High); 

        _unitOfWorkMock.Setup(x => x.Users.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _taskService.CreateTaskAsync(taskPostDto));
    }
    
    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_ShouldThrowInvalidDataException_WhenDueDateIsInPast()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var taskPostDto = new TaskPostDto ("Test Task", null, DateTime.Now.AddDays(-1), Status.Completed, Priority.High); 
        
        _unitOfWorkMock.Setup(x => x.Users.AnyAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() => _taskService.CreateTaskAsync(taskPostDto));
    }
    
    [Fact]
    public async System.Threading.Tasks.Task GetPagedTasksAsync_ShouldReturnTasks_WhenTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var pagedTasks = new PagedResponse<Task> 
        { 
            Items = new List<Task> { new Task { Id = Guid.NewGuid(), UserId = userId } }, 
            TotalPages = 1 
        };
        var pagedTaskDtos = new PagedResponse<TaskGetDto> 
        { 
            Items = new List<TaskGetDto> { new TaskGetDto(pagedTasks.Items.First().Id ,"title", null, null,
                Status.Completed,Priority.High, DateTime.Now, DateTime.Now)},
            TotalPages = 1 
        };
        
        var taskSortQueryDto = new TaskSortQueryDto(null, null, null, null);
        var taskPageQueryDto = new TaskPageQueryDto(1, 10);

        _unitOfWorkMock.Setup(x => x.Tasks.GetPagedTasksAsync(taskSortQueryDto, taskPageQueryDto, userId))
            .ReturnsAsync(pagedTasks);
        _mapperMock.Setup(x => x.Map<PagedResponse<TaskGetDto> >(pagedTasks)).Returns(pagedTaskDtos);

        // Act
        var result = await _taskService.GetPagedTasksAsync(taskSortQueryDto, taskPageQueryDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pagedTasks.Items.Count(), result.Items.Count());
    }
    
    [Fact]
    public async System.Threading.Tasks.Task GetPagedTasksAsync_ShouldThrowNotFoundException_WhenNoTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var emptyPagedTasks = new PagedResponse<Task> { Items = new List<Task>(), TotalPages = 0 };
        var taskSortQueryDto = new TaskSortQueryDto(null, null, null, null);
        var taskPageQueryDto = new TaskPageQueryDto(1, 10);

        _unitOfWorkMock.Setup(x => x.Tasks.GetPagedTasksAsync(taskSortQueryDto, taskPageQueryDto, userId))
            .ReturnsAsync(emptyPagedTasks);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _taskService.GetPagedTasksAsync(taskSortQueryDto, taskPageQueryDto));
    }
    
    [Fact]
    public async System.Threading.Tasks.Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var taskId = Guid.NewGuid();
        var task = new Task { Id = taskId, UserId = userId };
        var taskDto = new TaskGetDto(taskId, "title", null, null, Status.Completed, Priority.High, DateTime.Now,
            DateTime.Now);

        _unitOfWorkMock.Setup(x => x.Tasks.GetByIdAsync(taskId)).ReturnsAsync(task);
        _mapperMock.Setup(x => x.Map<TaskGetDto>(task)).Returns(taskDto);

        // Act
        var result = await _taskService.GetTaskByIdAsync(taskId);

        // Assert
        Assert.NotNull(result);
        _mapperMock.Verify(x => x.Map<TaskGetDto>(task), Times.Once);
    }
    
    [Fact]
    public async System.Threading.Tasks.Task GetTaskByIdAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var taskId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.Tasks.GetByIdAsync(taskId)).ReturnsAsync((Task)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _taskService.GetTaskByIdAsync(taskId));
    }
    
    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskAsync_ShouldUpdateTask_WhenTaskExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var taskId = Guid.NewGuid();
        var task = new Task { Id = taskId, UserId = userId };
        var taskPostDto = new TaskPostDto ("Updated Task", null, null, Status.Completed, Priority.Low); 

        _unitOfWorkMock.Setup(x => x.Tasks.GetByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        var result = await _taskService.UpdateTaskAsync(taskId, taskPostDto);

        // Assert
        Assert.Equal(taskId, result);
        _unitOfWorkMock.Verify(x => x.Tasks.Update(task), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var taskId = Guid.NewGuid();
        var taskPostDto = new TaskPostDto ("Updated Task", null, null, Status.Completed, Priority.Low); 

        _unitOfWorkMock.Setup(x => x.Tasks.GetByIdAsync(taskId)).ReturnsAsync((Task)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _taskService.UpdateTaskAsync(taskId, taskPostDto));
    }
    
    [Fact]
    public async System.Threading.Tasks.Task DeleteTaskAsync_ShouldDeleteTask_WhenTaskExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var taskId = Guid.NewGuid();
        var task = new Task { Id = taskId, UserId = userId };

        _unitOfWorkMock.Setup(x => x.Tasks.GetByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        await _taskService.DeleteTaskAsync(taskId);

        // Assert
        _unitOfWorkMock.Verify(x => x.Tasks.Remove(task), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async System.Threading.Tasks.Task DeleteTaskAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupHttpContextAccessor(userId);

        var taskId = Guid.NewGuid();

        _unitOfWorkMock.Setup(x => x.Tasks.GetByIdAsync(taskId)).ReturnsAsync((Task)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _taskService.DeleteTaskAsync(taskId));
    }
}





