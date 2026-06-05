using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DailyWorkReport.Models;

namespace DailyWorkReport.Data;

public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessWorkPattern>()
                    .HasKey(x => new 
                    { 
                        x.ProcessId, 
                        x.WorkPatternId 
                    });
    }

    public DbSet<Process> Processes { get; set; } = null!;
    public DbSet<ProcessWorkPattern> ProcessWorkPatterns { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductionOrder> ProductionOrders { get; set; } = null!;
    public DbSet<StandardWorkTime> StandardWorkTimes { get; set; } = null!;
    public DbSet<WorkClass> WorkClasses { get; set; } = null!;
    public DbSet<Worker> Workers { get; set; } = null!;
    public DbSet<WorkPattern> WorkPatterns { get; set; } = null!;
    public DbSet<WorkReport> WorkReports { get; set; } = null!;
    public DbSet<WorkReportWorker> WorkReportWorkers { get; set; } = null!;


}