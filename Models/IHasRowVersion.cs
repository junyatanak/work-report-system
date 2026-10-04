namespace DailyWorkReport.Models;

public interface IHasRowVersion
{
    Guid RowVersion { get; set; }
}