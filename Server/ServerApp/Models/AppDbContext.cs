using Microsoft.EntityFrameworkCore;

namespace ServerApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<UserAction> UserActions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Переконайтеся, що значення Price мають тип decimal (з суфіксом m)
            modelBuilder.Entity<Item>().HasData(
                new Item { Id = 1, Name = "Laptop", Quantity = 10, Price = 999.99m },
                new Item { Id = 2, Name = "Mouse", Quantity = 50, Price = 19.99m }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Email = "admin@warehouse.com" }
            );
        }
    }
}