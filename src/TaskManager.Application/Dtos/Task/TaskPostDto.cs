using TaskManager.Domain.Enums;

namespace TaskManager.Application.Dtos.Task;

public record TaskPostDto(string Title, string? Description, DateTime? DueDate, Status Status, Priority Priority);
