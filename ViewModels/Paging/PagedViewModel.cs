namespace DailyWorkReport.ViewModels.Paging;

public class PagedViewModel<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public PagingInfo Paging { get; init; } = new();
}