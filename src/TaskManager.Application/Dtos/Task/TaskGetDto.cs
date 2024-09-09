using Microsoft.VisualBasic;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Dtos.Task;

public record TaskGetDto(Guid Id, string Title, string? Description, DateTime? DueDate, Status Status,
    Priority Priority, DateTime CreatedAt, DateTime UpdatedAt);
