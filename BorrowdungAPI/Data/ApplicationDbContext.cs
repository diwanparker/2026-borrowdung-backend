using Microsoft.EntityFrameworkCore;
using BorrowdungAPI.Models.Entities;
using BorrowdungAPI.Models.Enums;

namespace BorrowdungAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users with BCrypt hashed passwords
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Email = "admin@pens.ac.id",
                    FullName = "Administrator",
                    Role = "Admin",
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Id = 2,
                    Username = "user",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Email = "user@pens.ac.id",
                    FullName = "Regular User",
                    Role = "User",
                    CreatedAt = DateTime.Now
                }
            );

            // Seed Rooms
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    Name = "Ruang Rapat A",
                    Capacity = 20,
                    Location = "Lantai 1, Gedung A",
                    Description = "Ruang rapat dengan fasilitas lengkap",
                    Status = "Tersedia",
                    CreatedAt = DateTime.Now
                },
                new Room
                {
                    Id = 2,
                    Name = "Ruang Rapat B",
                    Capacity = 15,
                    Location = "Lantai 2, Gedung A",
                    Description = "Ruang rapat medium dengan proyektor",
                    Status = "Tersedia",
                    CreatedAt = DateTime.Now
                },
                new Room
                {
                    Id = 3,
                    Name = "Aula",
                    Capacity = 100,
                    Location = "Lantai 3, Gedung B",
                    Description = "Aula besar untuk acara besar",
                    Status = "Tersedia",
                    CreatedAt = DateTime.Now
                }
            );

            // Seed Bookings
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    Id = 1,
                    RoomId = 1,
                    BookerName = "John Doe",
                    BookerEmail = "john@example.com",
                    BookerPhone = "081234567890",
                    Purpose = "Meeting with clients",
                    StartTime = DateTime.Now.AddDays(1),
                    EndTime = DateTime.Now.AddDays(1).AddHours(2),
                    Status = BookingStatus.Pending,
                    CreatedAt = DateTime.Now
                },
                new Booking
                {
                    Id = 2,
                    RoomId = 2,
                    BookerName = "Jane Smith",
                    BookerEmail = "jane@example.com",
                    BookerPhone = "081234567891",
                    Purpose = "Training session",
                    StartTime = DateTime.Now.AddDays(2),
                    EndTime = DateTime.Now.AddDays(2).AddHours(3),
                    Status = BookingStatus.Approved,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}
