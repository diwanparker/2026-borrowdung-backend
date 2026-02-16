# Borrowdung API - Backend

Borrowdung API adalah backend service untuk Sistem Peminjaman Ruangan Kampus yang dibangun menggunakan ASP.NET Core 8.0 dan Entity Framework Core. API ini menyediakan fitur manajemen ruangan, pencatatan peminjaman, dan pengelolaan status approval.

## Features

- **Room Management** - CRUD operations untuk mengelola data ruangan kampus
- **Booking Management** - Pencatatan dan pengelolaan peminjaman ruangan
- **Status Management** - Approval workflow (Pending → Approved/Rejected)
- **Conflict Detection** - Mencegah double booking pada ruangan yang sama
- **Booking History** - Riwayat dan penelusuran peminjaman
- **Soft Delete** - Data tidak benar-benar dihapus dari database
- **Search & Filter** - Pencarian dan filtering berdasarkan berbagai kriteria
- **Pagination** - Mendukung pagination untuk list data
- **Input Validation** - Validasi input menggunakan Data Annotations
- **RESTful API** - Mengikuti standar REST API best practices
- **Swagger/OpenAPI** - Dokumentasi API interaktif
- **CORS Support** - Mendukung akses dari frontend

## Tech Stack

- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: SQLite (file-based, mudah untuk development)
- **API Documentation**: Swagger/OpenAPI
- **Logging**: Built-in ASP.NET Core Logging

## Project Structure

```
BorrowdungAPI/
├── Controllers/          # API Controllers
│   ├── RoomController.cs       # Room CRUD endpoints
│   └── BookingController.cs    # Booking CRUD & status management
├── Data/                 # Database Context & Migrations
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Models/
│   ├── Entities/        # Database Entities
│   │   ├── Room.cs
│   │   └── Booking.cs
│   ├── Enums/
│   │   └── BookingStatus.cs
│   └── DTOs/            # Data Transfer Objects
│       ├── Room/        # Room DTOs
│       └── Booking/     # Booking DTOs
├── Properties/
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── .gitignore
```

## Installation

### Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Git](https://git-scm.com/downloads)

### Steps

1. **Clone the repository**

   ```bash
   git clone https://github.com/diwanparker/2026-borrowdung-backend.git
   cd 2026-borrowdung-backend/BorrowdungAPI
   ```

2. **Install dependencies**

   ```bash
   dotnet restore
   ```

3. **Run database migrations**

   ```bash
   dotnet ef database update
   ```

   Ini akan membuat database SQLite (`BorrowdungDB.db`) dan seed sample data (3 ruangan dan 2 booking).

4. **Run the application**

   ```bash
   dotnet run
   ```

   The API will be available at:
   - HTTP: http://localhost:5240
   - Swagger UI: http://localhost:5240/swagger

## Environment Variables

Configure the following environment variables in `appsettings.json`:

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | Database connection string | Data Source=BorrowdungDB.db |
| `ASPNETCORE_ENVIRONMENT` | Application environment | Development |
| `AllowedOrigins` | CORS allowed origins | * (AllowAll in development) |

## API Documentation

### Base URL

```
http://localhost:5240/api
```

### Endpoints

#### Room Management

- `GET /api/Room` - Get all rooms dengan filtering dan pagination
- `GET /api/Room/{id}` - Get room by ID dengan detail bookings
- `POST /api/Room` - Create new room
- `PUT /api/Room/{id}` - Update room
- `DELETE /api/Room/{id}` - Soft delete room

#### Booking Management

- `GET /api/Booking` - Get all bookings dengan filtering dan pagination
- `GET /api/Booking/{id}` - Get booking by ID
- `POST /api/Booking` - Create new booking (dengan conflict checking)
- `PUT /api/Booking/{id}` - Update booking (hanya untuk pending bookings)
- `PUT /api/Booking/{id}/status` - Update booking status (Approve/Reject)
- `DELETE /api/Booking/{id}` - Soft delete booking
- `GET /api/Booking/history` - Get booking history dengan filters

### Query Parameters

**GET `/api/Room` endpoint:**
- `search` - Search by name, location, atau description
- `status` - Filter by status (Tersedia/Tidak Tersedia)
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)

**GET `/api/Booking` endpoint:**
- `search` - Search by booker name, email, purpose, atau room name
- `status` - Filter by booking status (0=Pending, 1=Approved, 2=Rejected)
- `roomId` - Filter by room ID
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)

**GET `/api/Booking/history` endpoint:**
- Same parameters as GET `/api/Booking`

### Example Requests

**1. Get All Rooms:**

```bash
GET /api/Room?page=1&pageSize=10
```

Response:
```json
[
  {
    "id": 1,
    "name": "Ruang Seminar A",
    "location": "Gedung A Lantai 1",
    "capacity": 100,
    "description": "Ruang seminar besar dengan proyektor dan sound system",
    "status": "Tersedia",
    "createdAt": "2026-02-17T00:00:00",
    "bookings": []
  }
]
```

**2. Create New Booking:**

```bash
POST /api/Booking
Content-Type: application/json

{
  "roomId": 1,
  "bookerName": "John Doe",
  "bookerEmail": "john@pens.ac.id",
  "bookerPhone": "081234567890",
  "purpose": "Seminar teknologi AI",
  "startTime": "2026-02-25T09:00:00",
  "endTime": "2026-02-25T12:00:00"
}
```

Response:
```json
{
  "id": 3,
  "roomId": 1,
  "bookerName": "John Doe",
  "bookerEmail": "john@pens.ac.id",
  "bookerPhone": "081234567890",
  "purpose": "Seminar teknologi AI",
  "startTime": "2026-02-25T09:00:00",
  "endTime": "2026-02-25T12:00:00",
  "status": 0,
  "rejectionReason": null,
  "createdAt": "2026-02-17T10:30:00Z",
  "room": { ... }
}
```

**3. Approve Booking:**

```bash
PUT /api/Booking/3/status
Content-Type: application/json

{
  "status": 1
}
```

**4. Reject Booking with Reason:**

```bash
PUT /api/Booking/3/status
Content-Type: application/json

{
  "status": 2,
  "rejectionReason": "Ruangan sedang dalam perbaikan"
}
```

**5. Search Bookings:**

```bash
GET /api/Booking?search=seminar&status=1&page=1&pageSize=10
```

For interactive API documentation and testing, visit `/swagger` when running the application.

## Database Schema

### Rooms Table

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key (auto increment) |
| Name | string(100) | Room name (required) |
| Location | string(200) | Room location (required) |
| Capacity | int | Room capacity (required) |
| Description | string(500) | Room description (optional) |
| Status | string(20) | Room status (default: "Tersedia") |
| CreatedAt | datetime | Created timestamp |
| UpdatedAt | datetime | Updated timestamp (nullable) |
| DeletedAt | datetime | Deleted timestamp for soft delete (nullable) |

**Indexes:**
- Index on `Name` for faster searches

### Bookings Table

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key (auto increment) |
| RoomId | int | Foreign key to Rooms (required) |
| BookerName | string(100) | Booker name (required) |
| BookerEmail | string(100) | Booker email (required, validated) |
| BookerPhone | string(50) | Phone number (optional) |
| Purpose | string(500) | Booking purpose (required) |
| StartTime | datetime | Booking start time (required) |
| EndTime | datetime | Booking end time (required) |
| Status | enum | 0=Pending, 1=Approved, 2=Rejected (default: Pending) |
| RejectionReason | string(500) | Reason for rejection (optional) |
| CreatedAt | datetime | Created timestamp |
| UpdatedAt | datetime | Updated timestamp (nullable) |
| DeletedAt | datetime | Deleted timestamp for soft delete (nullable) |

**Indexes:**
- Index on `Status` for faster status filters
- Index on `BookerEmail` for faster searches
- Index on `RoomId` (foreign key)

**Relationships:**
- Booking → Room (Many-to-One)
- OnDelete: Restrict (prevent deleting room with bookings)

## Business Rules

1. **Conflict Detection:**
   - System checks for overlapping bookings when creating new booking
   - Only APPROVED bookings block time slots
   - Pending bookings don't prevent new bookings
   - Error message: "Room is already booked for this time period"

2. **Status Workflow:**
   - New bookings start with status: Pending
   - Can be updated to: Approved or Rejected
   - Rejection reason required when rejecting booking

3. **Update Restrictions:**
   - Can only update booking details if status is Pending
   - Approved/Rejected bookings cannot be modified

4. **Soft Delete:**
   - Deleted records kept in database with DeletedAt timestamp
   - Excluded from all query results
   - Data integrity preserved for audit purposes

## Usage

### Running in Development

```bash
dotnet run --environment Development
```

### Running in Production

```bash
dotnet run --environment Production
```

### Building for Production

```bash
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
```

## Testing

### Via Swagger UI (Recommended)

1. Run the application dan buka http://localhost:5240/swagger
2. Test creating a room:
   - Click `POST /api/Room` → Try it out
   - Input sample room data
3. Test creating a booking:
   - Click `POST /api/Booking` → Try it out
   - Input booking data with valid roomId
4. Test approval workflow:
   - Click `PUT /api/Booking/{id}/status` → Try it out
   - Approve or reject the booking
5. Test conflict detection:
   - Try creating overlapping booking for same room
   - Should receive error message

### Via cURL

**Get All Rooms:**
```bash
curl -X GET "http://localhost:5240/api/Room"
```

**Create New Booking:**
```bash
curl -X POST "http://localhost:5240/api/Booking" \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 1,
    "bookerName": "Test User",
    "bookerEmail": "test@pens.ac.id",
    "purpose": "Meeting",
    "startTime": "2026-02-25T10:00:00",
    "endTime": "2026-02-25T12:00:00"
  }'
```

**Approve Booking:**
```bash
curl -X PUT "http://localhost:5240/api/Booking/1/status" \
  -H "Content-Type: application/json" \
  -d '{"status": 1}'
```

### Via Postman

1. Import Swagger JSON dari http://localhost:5240/swagger/v1/swagger.json
2. Semua endpoints akan otomatis tersedia di Postman

## Sample Data

After running migrations, database will include:

**3 Sample Rooms:**
1. Ruang Seminar A (Gedung A Lantai 1) - Capacity: 100
2. Ruang Rapat B (Gedung B Lantai 2) - Capacity: 30
3. Lab Komputer 1 (Gedung C Lantai 3) - Capacity: 40

**2 Sample Bookings:**
1. Seminar Teknologi Informasi (Room 1) - Status: Approved
2. Rapat koordinasi proyek (Room 2) - Status: Pending

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes using Conventional Commits (`git commit -m 'feat(booking): add conflict detection'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Commit Message Format

Follow [Conventional Commits](https://www.conventionalcommits.org/):

- `feat(scope): description` - New feature
- `fix(scope): description` - Bug fix
- `docs(scope): description` - Documentation changes
- `refactor(scope): description` - Code refactoring
- `test(scope): description` - Adding tests
- `chore(scope): description` - Maintenance tasks

## License

This project is licensed under the MIT License.

## Credits

Developed by **PBL PENS Students** as part of Project-Based Learning course.

### Author Info

- **Institution**: Politeknik Elektronika Negeri Surabaya (PENS)
- **Course**: Project-Based Learning (PdBL)
- **Year**: 2026

## Contact

For questions or issues, please create an issue in the GitHub repository or contact the development team.

---

**Note**: This is an educational project developed for learning purposes as part of PENS PdBL coursework.
