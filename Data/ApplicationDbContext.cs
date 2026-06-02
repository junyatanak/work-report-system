using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DailyWorkReport.Data;

public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
{
    

}