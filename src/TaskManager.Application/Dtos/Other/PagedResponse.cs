namespace TaskManager.Application.Dtos.Other;

public class PagedResponse<T>
{
    // Total number of pages for the paginated response.
    public int TotalPages { get; set; }

    // Collection of items for the current page.
    public IEnumerable<T> Items { get; set; }
}
