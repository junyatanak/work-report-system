using Microsoft.AspNetCore.Identity;

namespace DailyWorkReport.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<WorkReport> WorkReports { get; } = new List<WorkReport>();
}