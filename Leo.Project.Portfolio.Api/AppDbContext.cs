using Leo.Project.Portfolio.Api.DbSet;
using Microsoft.EntityFrameworkCore;

namespace Leo.Project.Portfolio.Api;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Define your DbSet properties here (representing tables)
    public DbSet<RequestEmail> RequestEmails { get; set; }
}