namespace TaskManager.Application.Dtos.Task;

public class TaskQueryDto(int Page, int PageSize, string? Status, DateTime? DueDate, string? Priority, string? SortBy)
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Status { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Priority { get; set; }
    public string? SortBy { get; set; }
}