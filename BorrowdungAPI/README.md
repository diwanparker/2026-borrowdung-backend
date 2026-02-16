# Borrowdung API - Backend

Borrowdung API adalah backend service untuk sistem manajemen peminjaman yang dibangun menggunakan ASP.NET Core 8.0 dan Entity Framework Core. API ini menyediakan sistem autentikasi JWT dan manajemen user dengan role-based authorization.

## Features

- **JWT Authentication** - Autentikasi menggunakan JSON Web Token (HS256)
- **User Registration & Login** - Sistem pendaftaran dan login user
- **Role-Based Authorization** - Akses kontrol berdasarkan role (Admin & User)
- **User Management** - CRUD operations untuk manajemen user (Admin only)
- **Profile Management** - User dapat mengupdate profile dan password
- **Soft Delete** - Data tidak benar-benar dihapus dari database
- **Search & Filter** - Pencarian dan filtering user berdasarkan berbagai kriteria
- **Pagination** - Mendukung pagination untuk list data
- **Input Validation** - Validasi input menggunakan Data Annotations
- **Password Hashing** - BCrypt password hashing untuk keamanan
- **RESTful API** - Mengikuti standar REST API best practices
- **Swagger/OpenAPI** - Dokumentasi API interaktif dengan JWT authorization
- **CORS Support** - Mendukung akses dari frontend

## Tech Stack

- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: SQLite (file-based, mudah untuk development)
- **Authentication**: JWT Bearer Authentication
- **Password Hashing**: BCrypt.Net-Next
- **API Documentation**: Swagger/OpenAPI
- **Logging**: Built-in ASP.NET Core Logging

## Project Structure

```
BorrowdungAPI/
├── Controllers/          # API Controllers
│   ├── AuthController.cs      # Authentication endpoints
│   └── UserController.cs      # User management endpoints (Admin)
├── Data/                 # Database Context & Migrations
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Models/
│   ├── Entities/        # Database Entities
│   │   └── User.cs
│   ├── Enums/
│   │   └── UserRole.cs
│   └── DTOs/            # Data Transfer Objects
│       ├── Auth/        # Authentication DTOs
│       └── User/        # User management DTOs
├── Services/            # Business Logic Services
│   ├── IJwtService.cs
│   └── JwtService.cs
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

3. **Configure JWT Secret (Optional)**

   Edit `appsettings.json` dan update JWT configuration jika perlu:

   ```json
   {
     "Jwt": {
       "Secret": "YourSuperSecretKeyForJWTTokenGenerationThatIsAtLeast32CharactersLong",
       "Issuer": "BorrowdungAPI",
       "Audience": "BorrowdungClient",
       "ExpiryInMinutes": 1440
     }
   }
   ```

4. **Run database migrations**

   ```bash
   dotnet ef database update
   ```

   Ini akan membuat database SQLite (`BorrowdungDB.db`) dan seed default admin account.

5. **Run the application**

   ```bash
   dotnet run
   ```

   The API will be available at:
   - HTTP: http://localhost:5240
   - Swagger UI: http://localhost:5240/swagger

## Default Admin Account

Setelah database migration, akan ada default admin account:

- **Username**: `admin`
- **Password**: `Admin@123`
- **Email**: `admin@borrowdung.com`
- **Role**: `Admin`

**⚠️ PENTING**: Ubah password default ini saat pertama kali login di production!

## Environment Variables

Configure the following environment variables in `appsettings.json`:

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | Database connection string | Data Source=BorrowdungDB.db |
| `Jwt__Secret` | JWT signing secret key (min 32 chars) | YourSuperSecretKey... |
| `Jwt__Issuer` | JWT token issuer | BorrowdungAPI |
| `Jwt__Audience` | JWT token audience | BorrowdungClient |
| `Jwt__ExpiryInMinutes` | Token expiry time in minutes | 1440 (24 hours) |
| `ASPNETCORE_ENVIRONMENT` | Application environment | Development |
| `AllowedOrigins` | CORS allowed origins | http://localhost:3000 |

## API Documentation

### Base URL

```
http://localhost:5240/api
```

### Authentication

API ini menggunakan JWT Bearer Token authentication. Untuk mengakses protected endpoints:

1. Login melalui `/api/Auth/Login` untuk mendapatkan token
2. Sertakan token di header setiap request:
   ```
   Authorization: Bearer <your-token>
   ```

### Endpoints

#### Authentication (Public)

- `POST /api/Auth/Register` - Registrasi user baru
- `POST /api/Auth/Login` - Login dan dapatkan JWT token
- `GET /api/Auth/Profile` - Get profile user yang sedang login (Requires Auth)
- `PUT /api/Auth/Profile` - Update profile sendiri (Requires Auth)
- `PUT /api/Auth/ChangePassword` - Ubah password sendiri (Requires Auth)

#### User Management (Admin Only)

- `GET /api/User` - Get all users dengan filtering dan pagination
- `GET /api/User/{id}` - Get user by ID
- `POST /api/User` - Create new user dengan role selection
- `PUT /api/User/{id}` - Update user
- `DELETE /api/User/{id}` - Soft delete user

### Query Parameters

GET `/api/User` endpoint mendukung query parameters berikut:

- `search` - Search by username, email, atau fullName
- `role` - Filter by role (Admin/User)
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)

### Example Requests

**1. Register User Baru:**

```bash
POST /api/Auth/Register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john.doe@example.com",
  "password": "SecurePass@123",
  "confirmPassword": "SecurePass@123",
  "fullName": "John Doe",
  "phoneNumber": "081234567890"
}
```

**2. Login:**

```bash
POST /api/Auth/Login
Content-Type: application/json

{
  "usernameOrEmail": "admin",
  "password": "Admin@123"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "Bearer",
  "expiresAt": "2026-02-18T20:00:00Z",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@borrowdung.com",
    "fullName": "System Administrator",
    "role": "Admin"
  }
}
```

**3. Get Profile (Authenticated):**

```bash
GET /api/Auth/Profile
Authorization: Bearer <your-token>
```

**4. Get All Users (Admin Only):**

```bash
GET /api/User?page=1&pageSize=10&role=User
Authorization: Bearer <admin-token>
```

**5. Create User via Admin:**

```bash
POST /api/User
Authorization: Bearer <admin-token>
Content-Type: application/json

{
  "username": "newuser",
  "email": "new@example.com",
  "password": "NewPass@123",
  "fullName": "New User",
  "phoneNumber": "081234567891",
  "role": "User"
}
```

For interactive API documentation and testing, visit `/swagger` when running the application.

## Database Schema

### Users Table

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key (auto increment) |
| Username | string(50) | Username (unique, required) |
| Email | string(100) | Email address (unique, required) |
| Password | string(255) | BCrypt hashed password (required) |
| FullName | string(100) | Full name (required) |
| PhoneNumber | string(50) | Phone number (optional) |
| Role | enum | User role: 0=User, 1=Admin (default: User) |
| CreatedAt | datetime | Created timestamp |
| UpdatedAt | datetime | Updated timestamp (nullable) |
| DeletedAt | datetime | Deleted timestamp for soft delete (nullable) |

**Indexes:**
- Unique index on `Username`
- Unique index on `Email`

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
2. Test login dengan admin account:
   - Click `POST /api/Auth/Login` → Try it out
   - Input: `{"usernameOrEmail":"admin","password":"Admin@123"}`
   - Copy `token` dari response
3. Click tombol **Authorize** (gembok hijau) di pojok kanan atas
4. Input: `Bearer <paste-token-here>`
5. Sekarang bisa test semua protected endpoints

### Via cURL

**Login:**
```bash
curl -X POST "http://localhost:5240/api/Auth/Login" \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"admin","password":"Admin@123"}'
```

**Get Profile (with token):**
```bash
curl -X GET "http://localhost:5240/api/Auth/Profile" \
  -H "Authorization: Bearer <your-token>"
```

**Register New User:**
```bash
curl -X POST "http://localhost:5240/api/Auth/Register" \
  -H "Content-Type: application/json" \
  -d '{
    "username":"testuser",
    "email":"test@example.com",
    "password":"Test@123",
    "confirmPassword":"Test@123",
    "fullName":"Test User"
  }'
```

### Via Postman

1. Import Swagger JSON dari http://localhost:5240/swagger/v1/swagger.json
2. Semua endpoints akan otomatis tersedia di Postman

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes using Conventional Commits (`git commit -m 'feat(staff): add new validation'`)
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
- **Course**: Project-Based Learning
- **Year**: 2026

## Contact

For questions or issues, please create an issue in the GitHub repository or contact the development team.

---

**Note**: This is a educational project developed for learning purposes.
