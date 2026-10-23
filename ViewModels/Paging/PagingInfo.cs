using DocumentFormat.OpenXml.Wordprocessing;

namespace DailyWorkReport.ViewModels.Paging;

public class PagingInfo
{
    public const int DefaultPageSize = 20;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = DefaultPageSize;
    public int TotalCount { get; init; }

    public int TotalPages => Math.Max(1, (TotalCount + PageSize - 1) / PageSize);
}