using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BorrowdungAPI.Data;
using BorrowdungAPI.Models.Entities;
using BorrowdungAPI.Models.Enums;

namespace BorrowdungAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings(
            [FromQuery] string? search = null,
            [FromQuery] BookingStatus? status = null,
            [FromQuery] int? roomId = null)
        {
            var query = _context.Bookings
                .Include(b => b.Room)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.BookerName.Contains(search) ||
                    b.BookerEmail.Contains(search) ||
                    b.Purpose.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            if (roomId.HasValue)
            {
                query = query.Where(b => b.RoomId == roomId.Value);
            }

            var bookings = await query
                .Where(b => b.DeletedAt == null)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return Ok(bookings);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(CreateBookingDto dto)
        {
            // Check if room exists
            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null || room.DeletedAt != null)
            {
                return BadRequest(new { message = "Room not found" });
            }

            // Check for overlapping bookings
            var hasOverlap = await _context.Bookings
                .Where(b => b.RoomId == dto.RoomId &&
                           b.DeletedAt == null &&
                           b.Status != BookingStatus.Rejected &&
                           ((b.StartTime <= dto.StartTime && b.EndTime > dto.StartTime) ||
                            (b.StartTime < dto.EndTime && b.EndTime >= dto.EndTime) ||
                            (b.StartTime >= dto.StartTime && b.EndTime <= dto.EndTime)))
                .AnyAsync();

            if (hasOverlap)
            {
                return BadRequest(new { message = "Room is already booked for the selected time period" });
            }

            var booking = new Booking
            {
                RoomId = dto.RoomId,
                BookerName = dto.BookerName,
                BookerEmail = dto.BookerEmail,
                BookerPhone = dto.BookerPhone,
                Purpose = dto.Purpose,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Load the room data
            await _context.Entry(booking).Reference(b => b.Room).LoadAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        // PUT: api/Bookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, UpdateBookingDto dto)
        {
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null || existingBooking.DeletedAt != null)
            {
                return NotFound();
            }

            // Check for overlapping bookings (excluding this booking)
            var hasOverlap = await _context.Bookings
                .Where(b => b.RoomId == dto.RoomId &&
                           b.Id != id &&
                           b.DeletedAt == null &&
                           b.Status != BookingStatus.Rejected &&
                           ((b.StartTime <= dto.StartTime && b.EndTime > dto.StartTime) ||
                            (b.StartTime < dto.EndTime && b.EndTime >= dto.EndTime) ||
                            (b.StartTime >= dto.StartTime && b.EndTime <= dto.EndTime)))
                .AnyAsync();

            if (hasOverlap)
            {
                return BadRequest(new { message = "Room is already booked for the selected time period" });
            }

            existingBooking.RoomId = dto.RoomId;
            existingBooking.BookerName = dto.BookerName;
            existingBooking.BookerEmail = dto.BookerEmail;
            existingBooking.BookerPhone = dto.BookerPhone;
            existingBooking.Purpose = dto.Purpose;
            existingBooking.StartTime = dto.StartTime;
            existingBooking.EndTime = dto.EndTime;
            existingBooking.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // PATCH: api/Bookings/5/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null || booking.DeletedAt != null)
            {
                return NotFound();
            }

            booking.Status = request.Status;
            booking.RejectionReason = request.RejectionReason;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null || booking.DeletedAt != null)
            {
                return NotFound();
            }

            // Soft delete
            booking.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id && e.DeletedAt == null);
        }
    }

    public class CreateBookingDto
    {
        public int RoomId { get; set; }
        public string BookerName { get; set; } = string.Empty;
        public string BookerEmail { get; set; } = string.Empty;
        public string? BookerPhone { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class UpdateBookingDto
    {
        public int RoomId { get; set; }
        public string BookerName { get; set; } = string.Empty;
        public string BookerEmail { get; set; } = string.Empty;
        public string? BookerPhone { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class UpdateStatusRequest
    {
        public BookingStatus Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}
