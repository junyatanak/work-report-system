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


}