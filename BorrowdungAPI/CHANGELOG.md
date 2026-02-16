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
- JSON serialization with ReferenceHandler.IgnoreCycles to prevent circular references
- CORS configuration for cross-origin requests
- Swagger/OpenAPI documentation
- Logging and error handling

#### Database & Migrations
- Initial database migration with Room and Booking entities
- Database seeding with sample data:
  - 3 sample rooms (Ruang Seminar A, Ruang Rapat B, Lab Komputer 1)
  - 2 sample bookings (1 approved, 1 pending)
- Soft delete implementation for Room and Booking entities
- Indexes for performance optimization:
  - Index on Room.Name
  - Index on Booking.Status
  - Index on Booking.BookerEmail
  - Index on Booking.RoomId (foreign key)

#### Entities
- **Room Entity** with fields:
  - Id, Name, Location, Capacity, Description, Status
  - Timestamps: CreatedAt, UpdatedAt, DeletedAt
  - Navigation property: Bookings (collection)

- **Booking Entity** with fields:
  - Id, RoomId, BookerName, BookerEmail, BookerPhone
  - Purpose, StartTime, EndTime, Status, RejectionReason
  - Timestamps: CreatedAt, UpdatedAt, DeletedAt
  - Navigation property: Room

- **BookingStatus Enum**:
  - Pending = 0 (default)
  - Approved = 1
  - Rejected = 2

#### DTOs (Data Transfer Objects)
- **Room DTOs**:
  - CreateRoomRequest - Create new room with validation
  - UpdateRoomRequest - Update room details
- **Booking DTOs**:
  - CreateBookingRequest - Create booking with full details
  - UpdateBookingRequest - Update booking details (pending only)
  - UpdateBookingStatusRequest - Approve/reject with optional reason
- Input validation using Data Annotations
- Email format validation
- Required field validation
- String length validation

#### API Controllers
- **RoomController** - Room management endpoints:
  - GET /api/Room - List rooms with search, status filter, pagination
  - GET /api/Room/{id} - Get room with booking details
  - POST /api/Room - Create new room
  - PUT /api/Room/{id} - Update room
  - DELETE /api/Room/{id} - Soft delete room

- **BookingController** - Booking management endpoints:
  - GET /api/Booking - List bookings with search, filters, pagination
  - GET /api/Booking/{id} - Get booking details
  - POST /api/Booking - Create booking with conflict detection
  - PUT /api/Booking/{id} - Update booking (pending only)
  - PUT /api/Booking/{id}/status - Approve/reject booking
  - DELETE /api/Booking/{id} - Soft delete booking
  - GET /api/Booking/history - Get booking history with filters

#### Features
- **Conflict Detection**:
  - Prevents double-booking of rooms
  - Checks overlapping time periods
  - Only approved bookings block time slots
  - Pending bookings don't prevent new bookings

- **Status Management Workflow**:
  - New bookings created as Pending
  - Can be approved or rejected
  - Rejection reason required when rejecting
  - Status updates tracked with timestamps

- **Pagination Support**:
  - Configurable page size and page number
  - Default: page=1, pageSize=10
  - Pagination headers: X-Total-Count, X-Page, X-Page-Size

- **Search Functionality**:
  - Rooms: Search by name, location, description
  - Bookings: Search by booker name, email, purpose, room name

- **Filtering Options**:
  - Rooms: Filter by status (Tersedia/Tidak Tersedia)
  - Bookings: Filter by status (Pending/Approved/Rejected), roomId

- **Business Rules**:
  - Only pending bookings can be modified
  - Approved/rejected bookings are locked
  - Soft delete preserves data integrity
  - Foreign key constraint prevents room deletion with active bookings

- **Error Handling**:
  - Proper HTTP status codes (200, 201, 400, 404, 500)
  - Descriptive error messages in Indonesian
  - Exception logging
  - Validation error responses

- **Validation**:
  - Email format validation
  - Required field validation
  - Time period validation (start < end)
  - Room availability validation
  - Booking conflict validation

#### Documentation
- Comprehensive README.md with:
  - Project description and features
  - Tech stack information (ASP.NET Core 8.0, EF Core, SQLite)
  - Installation instructions
  - Database schema documentation
  - API endpoints documentation
  - Example requests and responses
  - Business rules explanation
  - Testing guide (Swagger UI, cURL, Postman)
  - Contributing guidelines with Conventional Commits
- .gitignore for .NET projects
- CHANGELOG.md following Keep a Changelog format

#### Development Tools
- Git repository initialization
- Environment configuration via appsettings.json
- Swagger UI for interactive API testing
- Database file: BorrowdungDB.db (SQLite)

### Changed
- N/A (Initial release)

### Deprecated
- N/A (Initial release)

### Removed
- N/A (Initial release)

### Fixed
- JSON circular reference issue resolved with ReferenceHandler.IgnoreCycles
- Header addition warnings (ASP0019) - using Response.Headers.Add for pagination headers

### Security
- Input validation to prevent malformed data
- Email format validation
- SQL injection prevention via EF Core parameterized queries
- Soft delete for data preservation and audit trail
- Foreign key constraints for data integrity

---

## Version History

- **v1.0.0** (2026-02-17) - Initial release with Room Booking Management System

---

## Semantic Versioning Guide

Given a version number MAJOR.MINOR.PATCH, increment the:

1. **MAJOR** version when you make incompatible API changes
2. **MINOR** version when you add functionality in a backward compatible manner
3. **PATCH** version when you make backward compatible bug fixes

Additional labels for pre-release and build metadata are available as extensions to the MAJOR.MINOR.PATCH format.

---

## Migration Notes

This is the initial release. No migration required.

---

## Features Roadmap

Future enhancements may include:
- User authentication and authorization
- Email notifications for booking approvals/rejections
- Calendar view integration
- Booking cancellation workflow
- Room equipment/facilities tracking
- Booking statistics and reports
- Mobile app integration

[Unreleased]: https://github.com/diwanparker/2026-borrowdung-backend/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/diwanparker/2026-borrowdung-backend/releases/tag/v1.0.0
