using Microsoft.EntityFrameworkCore;
using CRMBackend.Entities;

namespace CRMBackend;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<PlayerGame> PlayerGames { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerGame>()
            .HasKey(pg => new { pg.PlayerId, pg.GameId });

        base.OnModelCreating(modelBuilder);
    }
}
