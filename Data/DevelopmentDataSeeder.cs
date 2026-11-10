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

        if (!await db.Workers.AnyAsync())
        {
            for (var i = 1; i <= 10; i++)
            {
                db.Workers.Add(new Worker
                {
                    WorkerNumber = 1000 + i,
                    Name = i == 10 ? null : $"Worker {1000 + i}"
                });
            }
            await db.SaveChangesAsync();
        }

        var classes = await db.WorkClasses.ToDictionaryAsync(c => c.Name);

        if (!await db.Products.AnyAsync())
        {
            db.Products.AddRange(
                new Product { ProductCode = "10000001", Name = "Correction Tape A", WorkClassId = classes["CORRECTION-TAPE"].Id },
                new Product { ProductCode = "10000002", Name = "Correction Tape B", WorkClassId = classes["CORRECTION-TAPE"].Id }
            );
            await db.SaveChangesAsync();
        }
    }
}