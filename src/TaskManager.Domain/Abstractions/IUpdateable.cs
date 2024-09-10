namespace TaskManager.Domain.Abstractions;

public interface IUpdateable
{
    DateTime UpdatedAt { get; set; }
}