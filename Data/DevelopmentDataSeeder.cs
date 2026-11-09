using DailyWorkReport.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyWorkReport.Data;

/// <summary>
/// Development-only seeder (SQLite + EnsureCreated).
/// Idempotent: each table is seeded only when it is empty.
/// </summary>

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        await SeedMastersAsync(db);
        await SeedTransactionsAsync(db);
    }

    private static async Task SeedMastersAsync(ApplicationDbContext db)
    {
        if (!await db.WorkClasses.AnyAsync())
        {
            db.WorkClasses.AddRange(
                new WorkClass { Name = "CORRECTION-TAPE"},
                new WorkClass { Name = "ADHESIVE-TAPE"},
                new WorkClass { Name = "BALL-POINT-PEN"},
                new WorkClass { Name = "MECHANICAL-PENCIL"}
            );
            await db.SaveChangesAsync();
        }
    }
}