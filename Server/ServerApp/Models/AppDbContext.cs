using Microsoft.EntityFrameworkCore;

namespace ServerApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<UserAction> UserActions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування точності decimal для MySQL
            modelBuilder.Entity<Item>()
                .Property(i => i.Price)
                .HasPrecision(18, 2);

            // Налаштування зв'язку Foreign Key для UserAction
            modelBuilder.Entity<UserAction>()
                .HasOne(a => a.User)
                .WithMany(u => u.Actions)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Початкові дані (Seed Data)
            modelBuilder.Entity<Item>().HasData(
                new Item { Id = 1, Name = "Laptop", Quantity = 10, Price = 999.99m },
                new Item { Id = 2, Name = "Mouse", Quantity = 50, Price = 19.99m }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", FullName = "Admin User", Email = "arotar2005@gmail.com", Role = "Manager" }
            );
        }
    }
}
