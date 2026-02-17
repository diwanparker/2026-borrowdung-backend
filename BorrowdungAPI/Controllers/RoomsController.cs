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
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] int? minCapacity = null,
            [FromQuery] DateTime? startTime = null,
            [FromQuery] DateTime? endTime = null)
        {
            var query = _context.Rooms.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r =>
                    r.Name.Contains(search) ||
                    r.Location.Contains(search) ||
                    (r.Description != null && r.Description.Contains(search)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (minCapacity.HasValue)
            {
                query = query.Where(r => r.Capacity >= minCapacity.Value);
            }

            var rooms = await query
                .Where(r => r.DeletedAt == null)
                .OrderBy(r => r.Name)
                .ToListAsync();

            // If time range is provided, filter out rooms that are already booked
            if (startTime.HasValue && endTime.HasValue)
            {
                var bookedRoomIds = await _context.Bookings
                    .Where(b => b.DeletedAt == null &&
                               b.Status != BookingStatus.Rejected &&
                               ((b.StartTime <= startTime && b.EndTime > startTime) ||
                                (b.StartTime < endTime && b.EndTime >= endTime) ||
                                (b.StartTime >= startTime && b.EndTime <= endTime)))
                    .Select(b => b.RoomId)
                    .Distinct()
                    .ToListAsync();

                rooms = rooms.Where(r => !bookedRoomIds.Contains(r.Id)).ToList();
            }

            return Ok(rooms);
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        // POST: api/Rooms
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Room>> CreateRoom(Room room)
        {
            room.CreatedAt = DateTime.UtcNow;
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
        }

        // PUT: api/Rooms/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoom(int id, Room room)
        {
            if (id != room.Id)
            {
                return BadRequest();
            }

            var existingRoom = await _context.Rooms.FindAsync(id);
            if (existingRoom == null || existingRoom.DeletedAt != null)
            {
                return NotFound();
            }

            existingRoom.Name = room.Name;
            existingRoom.Capacity = room.Capacity;
            existingRoom.Location = room.Location;
            existingRoom.Description = room.Description;
            existingRoom.Status = room.Status;
            existingRoom.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null || room.DeletedAt != null)
            {
                return NotFound();
            }

            // Soft delete
            room.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id && e.DeletedAt == null);
        }
    }
}
