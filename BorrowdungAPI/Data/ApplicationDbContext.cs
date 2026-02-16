using Microsoft.EntityFrameworkCore;
using BorrowdungAPI.Models.Entities;
using BorrowdungAPI.Models.Enums;

namespace BorrowdungAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Room entity
            modelBuilder.Entity<Room>()
                .HasIndex(r => r.Name);

            // Configure Booking entity
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.Status);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookerEmail);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed sample rooms
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    Id = 1,
                    Name = "Ruang Seminar A",
                    Location = "Gedung A Lantai 1",
                    Capacity = 100,
                    Description = "Ruang seminar besar dengan proyektor dan sound system",
                    Status = "Tersedia",
                    CreatedAt = new DateTime(2026, 2, 17, 0, 0, 0, DateTimeKind.Utc)
                },
                new Room
                {
                    Id = 2,
                    Name = "Ruang Rapat B",
                    Location = "Gedung B Lantai 2",
                    Capacity = 30,
                    Description = "Ruang rapat dengan AC dan whiteboard",
                    Status = "Tersedia",
                    CreatedAt = new DateTime(2026, 2, 17, 0, 0, 0, DateTimeKind.Utc)
                },
                new Room
                {
                    Id = 3,
                    Name = "Lab Komputer 1",
                    Location = "Gedung C Lantai 3",
                    Capacity = 40,
                    Description = "Laboratorium komputer dengan 40 unit PC",
                    Status = "Tersedia",
                    CreatedAt = new DateTime(2026, 2, 17, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed sample bookings
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    Id = 1,
                    RoomId = 1,
                    BookerName = "Budi Santoso",
                    BookerEmail = "budi@pens.ac.id",
                    BookerPhone = "081234567890",
                    Purpose = "Seminar Teknologi Informasi",
                    StartTime = new DateTime(2026, 2, 20, 9, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2026, 2, 20, 12, 0, 0, DateTimeKind.Utc),
                    Status = BookingStatus.Approved,
                    CreatedAt = new DateTime(2026, 2, 17, 1, 0, 0, DateTimeKind.Utc)
                },
                new Booking
                {
                    Id = 2,
                    RoomId = 2,
                    BookerName = "Siti Nurhaliza",
                    BookerEmail = "siti@pens.ac.id",
                    BookerPhone = "081234567891",
                    Purpose = "Rapat koordinasi proyek",
                    StartTime = new DateTime(2026, 2, 21, 13, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2026, 2, 21, 15, 0, 0, DateTimeKind.Utc),
                    Status = BookingStatus.Pending,
                    CreatedAt = new DateTime(2026, 2, 17, 2, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
