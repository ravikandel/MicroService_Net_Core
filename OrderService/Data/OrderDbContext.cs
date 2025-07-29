using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // All tables will be created under the "Order" schema
            modelBuilder.HasDefaultSchema("Order");

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(d => d.Order)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add precision for decimal fields
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);  // Adjust as per your needs

            modelBuilder.Entity<OrderDetail>()
                .Property(d => d.Price)
                .HasPrecision(18, 2);  // Adjust as per your needs

            base.OnModelCreating(modelBuilder);
        }
    }
}
