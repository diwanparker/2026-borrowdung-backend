using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BorrowdungAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BookerEmail = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BookerPhone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Purpose = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RejectionReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Capacity", "CreatedAt", "DeletedAt", "Description", "Location", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 100, new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ruang seminar besar dengan proyektor dan sound system", "Gedung A Lantai 1", "Ruang Seminar A", "Tersedia", null },
                    { 2, 30, new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ruang rapat dengan AC dan whiteboard", "Gedung B Lantai 2", "Ruang Rapat B", "Tersedia", null },
                    { 3, 40, new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Laboratorium komputer dengan 40 unit PC", "Gedung C Lantai 3", "Lab Komputer 1", "Tersedia", null }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookerEmail", "BookerName", "BookerPhone", "CreatedAt", "DeletedAt", "EndTime", "Purpose", "RejectionReason", "RoomId", "StartTime", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "budi@pens.ac.id", "Budi Santoso", "081234567890", new DateTime(2026, 2, 17, 1, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 2, 20, 12, 0, 0, 0, DateTimeKind.Utc), "Seminar Teknologi Informasi", null, 1, new DateTime(2026, 2, 20, 9, 0, 0, 0, DateTimeKind.Utc), 1, null },
                    { 2, "siti@pens.ac.id", "Siti Nurhaliza", "081234567891", new DateTime(2026, 2, 17, 2, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 2, 21, 15, 0, 0, 0, DateTimeKind.Utc), "Rapat koordinasi proyek", null, 2, new DateTime(2026, 2, 21, 13, 0, 0, 0, DateTimeKind.Utc), 0, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookerEmail",
                table: "Bookings",
                column: "BookerEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Status",
                table: "Bookings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Name",
                table: "Rooms",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
