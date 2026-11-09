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

        if (!await db.Processes.AnyAsync())
        {
            db.Processes.AddRange(
                new Process { Name = "ASSEMBLY" },
                new Process { Name = "PACKING"},
                new Process { Name = "BLISTER-PACKING" },
                new Process { Name = "INKJET-PRINTING" },
                new Process { Name = "PAD-PRINTING" },
                new Process { Name = "SILK-SCREEN-PRINTING" },
                new Process { Name = "HEAT-TRANSFER-PRINTING" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.WorkPatterns.AnyAsync())
        {
            db.WorkPatterns.AddRange(
                new WorkPattern { Name = "BULK" },
                new WorkPattern { Name = "SLIDE-CARD-1P" },
                new WorkPattern { Name = "OPP-BAG-1P" },
                new WorkPattern { Name = "EXTRA-LABELING"},
                new WorkPattern { Name = "JAN-STICKER-1P"}
            );
            await db.SaveChangesAsync();
        }
    }
}