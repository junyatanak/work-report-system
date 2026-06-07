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
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProcessWorkPattern>()
                    .HasKey(x => new 
                    { 
                        x.ProcessId, 
                        x.WorkPatternId 
                    });
    }

    public DbSet<Process> Processes => Set<Process>();
    public DbSet<ProcessWorkPattern> ProcessWorkPatterns => Set<ProcessWorkPattern>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    public DbSet<StandardWorkTime> StandardWorkTimes => Set<StandardWorkTime>();
    public DbSet<WorkClass> WorkClasses => Set<WorkClass>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<WorkPattern> WorkPatterns => Set<WorkPattern>();
    public DbSet<WorkReport> WorkReports => Set<WorkReport>();
    public DbSet<WorkReportWorker> WorkReportWorkers => Set<WorkReportWorker>();


}