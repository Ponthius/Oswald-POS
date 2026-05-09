using Microsoft.EntityFrameworkCore;
using Oswald_POS.Models;

namespace Oswald_POS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Worker>().HasData(
                new Worker
                {
                    Id = 1,
                    FullName = "System Admin",
                    Username = "admin",
                    Password = "admin123",
                    Role = "Admin"
                }
            );
        }
    }
}