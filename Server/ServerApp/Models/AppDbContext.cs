using Microsoft.EntityFrameworkCore;

namespace ServerApp.Models
{
    /// <summary>
    /// Головний клас контексту бази даних для управління складом (MySQL).
    /// Забезпечує підключення, конфігурацію зв'язків та мапування сутностей системи.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Таблиця користувачів системи.
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Таблиця товарів на складі.
        /// </summary>
        public DbSet<Item> Items { get; set; } = null!;

        /// <summary>
        /// Таблиця історії операцій та дій над товарами.
        /// </summary>
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

            // Початкові дані (Seed Data) для тестування
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
