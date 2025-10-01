namespace PulseSystem.Application.DTOs.requests;

public class PageRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int Offset => (Page <= 1 ? 0 : (Page - 1)) * PageSize;

    public void EnsureValid(int maxPageSize = 200)
    {
        if (Page < 1) throw new ArgumentOutOfRangeException(nameof(Page), "Page must be >= 1.");
        if (PageSize < 1) throw new ArgumentOutOfRangeException(nameof(PageSize), "PageSize must be >= 1.");
        if (PageSize > maxPageSize) throw new ArgumentOutOfRangeException(nameof(PageSize), $"PageSize must be <= {maxPageSize}.");
    }
}