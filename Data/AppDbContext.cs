using Microsoft.EntityFrameworkCore;
using BlojayElectronics.Models;

namespace BlojayElectronics.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SQLite doesn't need HasPrecision – remove those lines

            // Seed categories (static data)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Smartphones" },
                new Category { Id = 2, Name = "Laptops" },
                new Category { Id = 3, Name = "Audio & Headphones" },
                new Category { Id = 4, Name = "Accessories" }
            );

            // No product seed – products added by admin
        }
    }
}