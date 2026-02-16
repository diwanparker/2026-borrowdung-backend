using Microsoft.EntityFrameworkCore;
using BorrowdungAPI.Models.Entities;

namespace BorrowdungAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure indexes for better query performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed default admin user
            var adminPassword = BCrypt.Net.BCrypt.HashPassword("Admin@123");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@borrowdung.com",
                    Password = adminPassword,
                    FullName = "System Administrator",
                    PhoneNumber = "081234567890",
                    Role = Models.Enums.UserRole.Admin,
                    CreatedAt = new DateTime(2026, 2, 17, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
