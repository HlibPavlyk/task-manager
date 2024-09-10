using System.ComponentModel;

namespace TaskManager.Application.Dtos.Task;

public record TaskSortQueryDto(string? Status, DateTime? DueDate, string? Priority, string? SortBy);