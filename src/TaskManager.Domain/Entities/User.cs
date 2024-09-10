using TaskManager.Domain.Abstractions;

namespace TaskManager.Domain.Entities;

public class User : IUpdateable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;  // Password will be hashed
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public ICollection<Task> Tasks { get; set; }
}