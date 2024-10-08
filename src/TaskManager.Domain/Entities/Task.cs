using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class Task : IUpdateable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Status Status { get; set; } = Status.Pending;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign key to User
    public Guid UserId { get; set; } 
    public User User { get; set; } // Navigation property
}
