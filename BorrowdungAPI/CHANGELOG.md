# Changelog

All notable changes to the Borrowdung API project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-02-17

### Added

#### Core Infrastructure
- Initial project setup with ASP.NET Core 8.0
- Entity Framework Core 8.0 configuration with SQLite support
- Database context with ApplicationDbContext
- JWT Bearer Authentication implementation
- Role-based authorization (Admin & User roles)
- CORS configuration for cross-origin requests
- Swagger/OpenAPI documentation with JWT authorization support

#### Authentication & Security
- JWT token generation using HS256 algorithm
- BCrypt password hashing for secure password storage
- Token expiry configuration (default: 24 hours)
- JWT claims: Sub, Email, NameIdentifier, Name, Role, FullName
- Bearer token authentication middleware
- Authorization policies: AdminOnly, UserOnly, AllUsers

#### Database & Migrations
- Initial database migration with User entity
- Database seeding with default admin account:
  - Username: `admin`
  - Email: `admin@borrowdung.com`
  - Password: `Admin@123` (BCrypt hashed)
  - Role: Admin
- Soft delete implementation for User entity
- Unique constraints on Username and Email fields

#### Entities
- **User Entity** with fields:
  - Id, Username, Email, Password (BCrypt hashed)
  - FullName, PhoneNumber, Role (Admin/User enum)
  - Timestamps: CreatedAt, UpdatedAt, DeletedAt

#### DTOs (Data Transfer Objects)
- **Auth DTOs**:
  - RegisterRequest - User registration with password confirmation
  - LoginRequest - Login dengan username atau email
  - LoginResponse - JWT token dengan user info
  - UserResponse - User data tanpa password
- **User Management DTOs**:
  - CreateUserRequest - Admin creates user dengan role selection
  - UpdateUserRequest - Update user profile
  - ChangePasswordRequest - Password change dengan current password verification
- Input validation using Data Annotations
- Email format validation
- Password strength validation (min 6 chars, requires uppercase, lowercase, digit, special char)
- Password confirmation matching

#### Services
- **JwtService** - JWT token generation and management:
  - Generate JWT tokens with custom claims
  - Configurable token expiry
  - Token validation parameters

#### API Controllers
- **AuthController** - Public authentication endpoints:
  - POST /api/Auth/Register - User registration (default role: User)
  - POST /api/Auth/Login - Login dengan username/email, returns JWT token
  - GET /api/Auth/Profile - Get current user profile (Authenticated)
  - PUT /api/Auth/Profile - Update own profile (Authenticated)
  - PUT /api/Auth/ChangePassword - Change password (Authenticated)

- **UserController** - Admin-only user management:
  - GET /api/User - List users dengan pagination, search, role filter
  - GET /api/User/{id} - Get user by ID
  - POST /api/User - Create user dengan role selection
  - PUT /api/User/{id} - Update user
  - DELETE /api/User/{id} - Soft delete user (dengan self-deletion prevention)

#### Features
- **Pagination Support**:
  - Configurable page size dan page number
  - Default: page=1, pageSize=10
- **Search Functionality**:
  - Search by username, email, atau fullName
- **Filtering Options**:
  - Filter by role (Admin/User)
- **Authorization**:
  - Admin dapat akses semua endpoints
  - User hanya dapat akses profile sendiri
  - Admin tidak bisa delete diri sendiri
- **Error Handling**:
  - Proper HTTP status codes (200, 201, 400, 401, 403, 404)
  - Descriptive error messages
  - Exception logging
- **Validation**:
  - Duplicate username prevention
  - Duplicate email prevention
  - Password match validation
  - Current password verification saat change password
  - Self-deletion prevention untuk admin

#### Documentation
- Comprehensive README.md with:
  - Project description dan features
  - Tech stack information (SQLite, JWT, BCrypt)
  - Installation instructions
  - Default admin credentials warning
  - JWT authentication guide
  - API endpoints documentation
  - Example requests dengan authentication
  - Database schema
  - Testing guide (Swagger UI, cURL, Postman)
  - Contributing guidelines dengan Conventional Commits
- .gitignore for .NET projects
- CHANGELOG.md following Keep a Changelog format

#### Development Tools
- Git repository initialization
- Environment configuration via appsettings.json
- Swagger UI untuk interactive API testing
- JWT token testing via Swagger authorization

### Changed
- N/A (Initial release)

### Deprecated
- N/A (Initial release)

### Removed
- N/A (Initial release)

### Fixed
- N/A (Initial release)

### Security
- JWT Bearer token authentication untuk semua protected endpoints
- BCrypt password hashing dengan automatic salting
- Password strength requirements
- Username dan email uniqueness validation
- Role-based authorization dengan Admin dan User roles
- Soft delete untuk preserve data integrity
- Current password verification sebelum change password
- Input validation untuk prevent malformed data
- Self-deletion prevention untuk admin users

---

## Version History

- **v1.0.0** (2026-02-17) - Initial release dengan JWT authentication, user registration, dan role-based authorization

---

## Semantic Versioning Guide

Given a version number MAJOR.MINOR.PATCH, increment the:

1. **MAJOR** version when you make incompatible API changes
2. **MINOR** version when you add functionality in a backward compatible manner
3. **PATCH** version when you make backward compatible bug fixes

Additional labels for pre-release and build metadata are available as extensions to the MAJOR.MINOR.PATCH format.

[Unreleased]: https://github.com/diwanparker/2026-borrowdung-backend/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/diwanparker/2026-borrowdung-backend/releases/tag/v1.0.0
