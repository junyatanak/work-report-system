using DocumentFormat.OpenXml.Wordprocessing;

namespace DailyWorkReport.ViewModels.Paging;

public class PagingInfo
{
    public const int DefaultPageSize = 20;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = DefaultPageSize;
    public int TotalCount { get; init; }

    public int TotalPages => Math.Max(1, (TotalCount + PageSize - 1) / PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
    public int FirstItem => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;
    public int LastItem => Math.Min(Page * PageSize, TotalCount);
}