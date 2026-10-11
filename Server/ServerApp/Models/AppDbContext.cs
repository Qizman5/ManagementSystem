using Microsoft.EntityFrameworkCore;

namespace ServerApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Worker> Workers { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<UserAction> UserActions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Worker>().ToTable("Workers");
            modelBuilder.Entity<Item>().ToTable("Products");
            modelBuilder.Entity<UserAction>().ToTable("Actions");
        }
    }
}
