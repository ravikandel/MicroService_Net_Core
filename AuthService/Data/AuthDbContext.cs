using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // All tables will be created under the "Auth" schema
            modelBuilder.HasDefaultSchema("Auth");
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
             modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
            base.OnModelCreating(modelBuilder);
        }
    }
}
