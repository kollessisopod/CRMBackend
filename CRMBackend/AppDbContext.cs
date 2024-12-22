using Microsoft.EntityFrameworkCore;
using CRMBackend.Entities;

namespace CRMBackend;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Player> Players { get; set; }
}
