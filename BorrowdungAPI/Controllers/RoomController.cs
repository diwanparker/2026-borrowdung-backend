using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BorrowdungAPI.Data;
using BorrowdungAPI.Models.Entities;
using BorrowdungAPI.Models.DTOs.Room;

namespace BorrowdungAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Room
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Rooms
                    .Where(r => r.DeletedAt == null);

                // Search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(r =>
                        r.Name.Contains(search) ||
                        r.Location.Contains(search) ||
                        (r.Description != null && r.Description.Contains(search)));
                }

                // Status filter
                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(r => r.Status == status);
                }

                var totalCount = await query.CountAsync();
                var rooms = await query
                    .OrderBy(r => r.Name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Page", page.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving rooms", error = ex.Message });
            }
        }

        // GET: api/Room/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            try
            {
                var room = await _context.Rooms
                    .Include(r => r.Bookings.Where(b => b.DeletedAt == null))
                    .FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);

                if (room == null)
                {
                    return NotFound(new { message = "Room not found" });
                }

                return Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving room", error = ex.Message });
            }
        }

        // POST: api/Room
        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom(CreateRoomRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var room = new Room
                {
                    Name = request.Name,
                    Location = request.Location,
                    Capacity = request.Capacity,
                    Description = request.Description,
                    Status = request.Status,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating room", error = ex.Message });
            }
        }

        // PUT: api/Room/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, UpdateRoomRequest request)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(id);

                if (room == null || room.DeletedAt != null)
                {
                    return NotFound(new { message = "Room not found" });
                }

                if (request.Name != null) room.Name = request.Name;
                if (request.Location != null) room.Location = request.Location;
                if (request.Capacity.HasValue) room.Capacity = request.Capacity.Value;
                if (request.Description != null) room.Description = request.Description;
                if (request.Status != null) room.Status = request.Status;

                room.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(room);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating room", error = ex.Message });
            }
        }

        // DELETE: api/Room/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(id);

                if (room == null || room.DeletedAt != null)
                {
                    return NotFound(new { message = "Room not found" });
                }

                // Soft delete
                room.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Room deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting room", error = ex.Message });
            }
        }
    }
}
