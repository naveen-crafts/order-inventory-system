using Microsoft.EntityFrameworkCore;

namespace OrderInventory.Api;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        b.Entity<Order>().Property(o => o.Total).HasPrecision(18, 2);
        b.Entity<OrderItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);
        b.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Keyboard", Price = 1500m, Stock = 50 },
            new Product { Id = 2, Name = "Mouse", Price = 700m, Stock = 80 },
            new Product { Id = 3, Name = "Monitor", Price = 9500m, Stock = 20 });
    }
}
