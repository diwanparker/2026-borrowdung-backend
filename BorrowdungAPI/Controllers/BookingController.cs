using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BorrowdungAPI.Data;
using BorrowdungAPI.Models.Entities;
using BorrowdungAPI.Models.DTOs.Booking;
using BorrowdungAPI.Models.Enums;

namespace BorrowdungAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings(
            [FromQuery] string? search,
            [FromQuery] BookingStatus? status,
            [FromQuery] int? roomId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Bookings
                    .Include(b => b.Room)
                    .Where(b => b.DeletedAt == null);

                // Search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(b =>
                        b.BookerName.Contains(search) ||
                        b.BookerEmail.Contains(search) ||
                        b.Purpose.Contains(search) ||
                        b.Room.Name.Contains(search));
                }

                // Status filter
                if (status.HasValue)
                {
                    query = query.Where(b => b.Status == status.Value);
                }

                // Room filter
                if (roomId.HasValue)
                {
                    query = query.Where(b => b.RoomId == roomId.Value);
                }

                var totalCount = await query.CountAsync();
                var bookings = await query
                    .OrderByDescending(b => b.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Page", page.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving bookings", error = ex.Message });
            }
        }

        // GET: api/Booking/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Room)
                    .FirstOrDefaultAsync(b => b.Id == id && b.DeletedAt == null);

                if (booking == null)
                {
                    return NotFound(new { message = "Booking not found" });
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving booking", error = ex.Message });
            }
        }

        // POST: api/Booking
        [HttpPost]
        public async Task<ActionResult<Booking>> CreateBooking(CreateBookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate room exists
                var room = await _context.Rooms.FindAsync(request.RoomId);
                if (room == null || room.DeletedAt != null)
                {
                    return BadRequest(new { message = "Room not found" });
                }

                // Validate time range
                if (request.StartTime >= request.EndTime)
                {
                    return BadRequest(new { message = "End time must be after start time" });
                }

                // Check for conflicting bookings (only approved bookings)
                var hasConflict = await _context.Bookings
                    .AnyAsync(b =>
                        b.RoomId == request.RoomId &&
                        b.DeletedAt == null &&
                        b.Status == BookingStatus.Approved &&
                        ((request.StartTime >= b.StartTime && request.StartTime < b.EndTime) ||
                         (request.EndTime > b.StartTime && request.EndTime <= b.EndTime) ||
                         (request.StartTime <= b.StartTime && request.EndTime >= b.EndTime)));

                if (hasConflict)
                {
                    return BadRequest(new { message = "Room is already booked for this time period" });
                }

                var booking = new Booking
                {
                    RoomId = request.RoomId,
                    BookerName = request.BookerName,
                    BookerEmail = request.BookerEmail,
                    BookerPhone = request.BookerPhone,
                    Purpose = request.Purpose,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Status = BookingStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Load room data for response
                await _context.Entry(booking).Reference(b => b.Room).LoadAsync();

                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating booking", error = ex.Message });
            }
        }

        // PUT: api/Booking/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, UpdateBookingRequest request)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null || booking.DeletedAt != null)
                {
                    return NotFound(new { message = "Booking not found" });
                }

                // Only allow updates for pending bookings
                if (booking.Status != BookingStatus.Pending)
                {
                    return BadRequest(new { message = "Can only update pending bookings" });
                }

                if (request.RoomId.HasValue)
                {
                    var room = await _context.Rooms.FindAsync(request.RoomId.Value);
                    if (room == null || room.DeletedAt != null)
                    {
                        return BadRequest(new { message = "Room not found" });
                    }
                    booking.RoomId = request.RoomId.Value;
                }

                if (request.BookerName != null) booking.BookerName = request.BookerName;
                if (request.BookerEmail != null) booking.BookerEmail = request.BookerEmail;
                if (request.BookerPhone != null) booking.BookerPhone = request.BookerPhone;
                if (request.Purpose != null) booking.Purpose = request.Purpose;
                if (request.StartTime.HasValue) booking.StartTime = request.StartTime.Value;
                if (request.EndTime.HasValue) booking.EndTime = request.EndTime.Value;

                // Validate time range if changed
                if (booking.StartTime >= booking.EndTime)
                {
                    return BadRequest(new { message = "End time must be after start time" });
                }

                booking.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _context.Entry(booking).Reference(b => b.Room).LoadAsync();

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating booking", error = ex.Message });
            }
        }

        // PUT: api/Booking/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, UpdateBookingStatusRequest request)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null || booking.DeletedAt != null)
                {
                    return NotFound(new { message = "Booking not found" });
                }

                // If approving, check for conflicts
                if (request.Status == BookingStatus.Approved)
                {
                    var hasConflict = await _context.Bookings
                        .AnyAsync(b =>
                            b.Id != id &&
                            b.RoomId == booking.RoomId &&
                            b.DeletedAt == null &&
                            b.Status == BookingStatus.Approved &&
                            ((booking.StartTime >= b.StartTime && booking.StartTime < b.EndTime) ||
                             (booking.EndTime > b.StartTime && booking.EndTime <= b.EndTime) ||
                             (booking.StartTime <= b.StartTime && booking.EndTime >= b.EndTime)));

                    if (hasConflict)
                    {
                        return BadRequest(new { message = "Room is already booked for this time period" });
                    }
                }

                // If rejecting, require reason
                if (request.Status == BookingStatus.Rejected && string.IsNullOrWhiteSpace(request.RejectionReason))
                {
                    return BadRequest(new { message = "Rejection reason is required" });
                }

                booking.Status = request.Status;
                booking.RejectionReason = request.RejectionReason;
                booking.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _context.Entry(booking).Reference(b => b.Room).LoadAsync();

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating booking status", error = ex.Message });
            }
        }

        // DELETE: api/Booking/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null || booking.DeletedAt != null)
                {
                    return NotFound(new { message = "Booking not found" });
                }

                // Soft delete
                booking.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Booking deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting booking", error = ex.Message });
            }
        }

        // GET: api/Booking/history
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingHistory(
            [FromQuery] string? email,
            [FromQuery] int? roomId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Bookings
                    .Include(b => b.Room)
                    .Where(b => b.DeletedAt == null);

                if (!string.IsNullOrWhiteSpace(email))
                {
                    query = query.Where(b => b.BookerEmail == email);
                }

                if (roomId.HasValue)
                {
                    query = query.Where(b => b.RoomId == roomId.Value);
                }

                var totalCount = await query.CountAsync();
                var bookings = await query
                    .OrderByDescending(b => b.StartTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Page", page.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving booking history", error = ex.Message });
            }
        }
    }
}
