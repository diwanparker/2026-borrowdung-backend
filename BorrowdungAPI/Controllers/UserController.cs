using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BorrowdungAPI.Data;
using BorrowdungAPI.Models.Entities;
using BorrowdungAPI.Models.DTOs.Auth;
using BorrowdungAPI.Models.DTOs.User;

namespace BorrowdungAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers(
            [FromQuery] string? search,
            [FromQuery] string? role,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Users
                    .Where(u => u.DeletedAt == null)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(u =>
                        u.Username.Contains(search) ||
                        u.Email.Contains(search) ||
                        u.FullName.Contains(search));
                }

                // Apply role filter
                if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<Models.Enums.UserRole>(role, true, out var userRole))
                {
                    query = query.Where(u => u.Role == userRole);
                }

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var users = await query
                    .OrderBy(u => u.Username)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserResponse
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        FullName = u.FullName,
                        PhoneNumber = u.PhoneNumber,
                        Role = u.Role.ToString(),
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                Response.Headers.Add("X-Page", page.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users list");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data users" });
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == id && u.DeletedAt == null)
                    .Select(u => new UserResponse
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        FullName = u.FullName,
                        PhoneNumber = u.PhoneNumber,
                        Role = u.Role.ToString(),
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new { message = $"User dengan ID {id} tidak ditemukan" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user with ID {Id}", id);
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data user" });
            }
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest request)
        {
            try
            {
                // Check if username already exists
                var usernameExists = await _context.Users
                    .AnyAsync(u => u.Username == request.Username && u.DeletedAt == null);

                if (usernameExists)
                {
                    return BadRequest(new { message = "Username sudah terdaftar" });
                }

                // Check if email already exists
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == request.Email && u.DeletedAt == null);

                if (emailExists)
                {
                    return BadRequest(new { message = "Email sudah terdaftar" });
                }

                // Hash password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = hashedPassword,
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    Role = request.Role,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var response = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role.ToString(),
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(500, new { message = "Terjadi kesalahan saat membuat user" });
            }
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

                if (user == null)
                {
                    return NotFound(new { message = $"User dengan ID {id} tidak ditemukan" });
                }

                // Check if email already exists (excluding current user)
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == request.Email && u.Id != id && u.DeletedAt == null);

                if (emailExists)
                {
                    return BadRequest(new { message = "Email sudah terdaftar" });
                }

                user.Email = request.Email;
                user.FullName = request.FullName;
                user.PhoneNumber = request.PhoneNumber;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Data user berhasil diperbarui" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {Id}", id);
                return StatusCode(500, new { message = "Terjadi kesalahan saat memperbarui data user" });
            }
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

                if (user == null)
                {
                    return NotFound(new { message = $"User dengan ID {id} tidak ditemukan" });
                }

                // Prevent deleting yourself
                var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (user.Id == currentUserId)
                {
                    return BadRequest(new { message = "Tidak dapat menghapus akun sendiri" });
                }

                // Soft delete
                user.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "User berhasil dihapus" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {Id}", id);
                return StatusCode(500, new { message = "Terjadi kesalahan saat menghapus user" });
            }
        }
    }
}
