using DailyWorkReport.Models;
using DocumentFormat.OpenXml.Office.Excel;
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
                new WorkPattern { Name = "JAN-STICKER-1P"},
                new WorkPattern { Name = "STANDARD"}
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
                new Product { ProductCode = "10000002", Name = "Correction Tape B", WorkClassId = classes["CORRECTION-TAPE"].Id },
                new Product { ProductCode = "20000001", Name = "Adhesive Tape A", WorkClassId = classes["ADHESIVE-TAPE"].Id },
                new Product { ProductCode = "20000002", Name = "Adhesive Tape B", WorkClassId = classes["ADHESIVE-TAPE"].Id },
                new Product { ProductCode = "30000001", Name = "Ball Point Pen A", WorkClassId = classes["BALL-POINT-PEN"].Id },
                new Product { ProductCode = "40000001", Name = "Mechanical Pencil A", WorkClassId = classes["MECHANICAL-PENCIL"].Id }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.StandardWorkTimes.AnyAsync())
        {
            var processes = await db.Processes.ToDictionaryAsync(p => p.Name);
            var patterns = await db.WorkPatterns.ToDictionaryAsync(p => p.Name);

            var rows = new (string Class, string Process, string Pattern, decimal? Seconds)[]
            {
                ("CORRECTION-TAPE", "ASSEMBLY", "BULK", 80m),
                ("CORRECTION-TAPE", "ASSEMBLY", "SLIDE-CARD-1P", 100m),
                ("CORRECTION-TAPE", "ASSEMBLY", "OPP-BAG-1P", 100m),
                ("CORRECTION-TAPE", "ASSEMBLY", "EXTRA-LABELING", 120m),
                ("ADHESIVE-TAPE", "ASSEMBLY", "BULK", 70m),
                ("ADHESIVE-TAPE", "ASSEMBLY", "SLIDE-CARD-1P", 100),
                ("ADHESIVE-TAPE", "PACKING", "OPP-BAG-1P", 120m),
                ("BALL-POINT-PEN", "ASSEMBLY", "BULK", 30m),
                ("BALL-POINT-PEN", "HEAT-TRANSFER-PRINTING", "STANDARD", 10m),
                ("MECHANICAL-PENCIL", "BLISTER-PACKING", "STANDARD", 10m)
            };

            foreach (var r in rows)
            {
                db.StandardWorkTimes.Add(new StandardWorkTime
                {
                    WorkClassId = classes[r.Class].Id,
                    ProcessId = processes[r.Process].Id,
                    WorkPatternId = patterns[r.Pattern].Id,
                    StandardCycleSeconds = r.Seconds
                });
            }
            await db.SaveChangesAsync();
        }
    }

    private static async Task SeedTransactionsAsync(ApplicationDbContext db)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (!await db.ProductionOrders.AnyAsync())
        {
            var products = await db.Products.OrderBy(p => p.ProductCode).ToListAsync();

            for (var i = 0; i < 10; i++)
            {
                var product = products[i % products.Count];
                db.ProductionOrders.Add(new ProductionOrder
                {
                    OrderNumber = $"PO-{i + 1:D4}",
                    ProductId = product.Id,
                    OrderQty = 1000 + (i * 100),
                    DueDate = today.AddDays(7 + i)
                });
            }

            await db.SaveChangesAsync();
        }

        if (!await db.WorkReports.AnyAsync())
        {
            
        }
    }


}