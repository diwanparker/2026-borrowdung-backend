using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BorrowdungAPI.Data;
using BorrowdungAPI.Models.Entities;
using BorrowdungAPI.Models.DTOs.Auth;
using BorrowdungAPI.Models.DTOs.User;
using BorrowdungAPI.Services;
using System.Security.Claims;

namespace BorrowdungAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ApplicationDbContext context,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        // POST: api/Auth/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
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
                    Role = Models.Enums.UserRole.User, // Default role
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = _jwtService.GenerateToken(user);
                var expiresAt = _jwtService.GetTokenExpiry();

                var response = new LoginResponse
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    User = new UserResponse
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role.ToString(),
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mendaftar" });
            }
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            try
            {
                // Find user by username or email
                var user = await _context.Users
                    .Where(u => (u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail)
                               && u.DeletedAt == null)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return Unauthorized(new { message = "Username/email atau password salah" });
                }

                // Verify password
                var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

                if (!isPasswordValid)
                {
                    return Unauthorized(new { message = "Username/email atau password salah" });
                }

                // Generate JWT token
                var token = _jwtService.GenerateToken(user);
                var expiresAt = _jwtService.GetTokenExpiry();

                var response = new LoginResponse
                {
                    Token = token,
                    ExpiresAt = expiresAt,
                    User = new UserResponse
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        FullName = user.FullName,
                        PhoneNumber = user.PhoneNumber,
                        Role = user.Role.ToString(),
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(500, new { message = "Terjadi kesalahan saat login" });
            }
        }

        // GET: api/Auth/Profile
        [HttpGet("Profile")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var user = await _context.Users
                    .Where(u => u.Id == userId && u.DeletedAt == null)
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
                    return NotFound(new { message = "User tidak ditemukan" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user profile");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil profil" });
            }
        }

        // PUT: api/Auth/Profile
        [HttpPut("Profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateUserRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);

                if (user == null)
                {
                    return NotFound(new { message = "User tidak ditemukan" });
                }

                // Check if email already exists (excluding current user)
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == request.Email && u.Id != userId && u.DeletedAt == null);

                if (emailExists)
                {
                    return BadRequest(new { message = "Email sudah terdaftar" });
                }

                user.Email = request.Email;
                user.FullName = request.FullName;
                user.PhoneNumber = request.PhoneNumber;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Profil berhasil diperbarui" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating profile");
                return StatusCode(500, new { message = "Terjadi kesalahan saat memperbarui profil" });
            }
        }

        // PUT: api/Auth/ChangePassword
        [HttpPut("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt == null);

                if (user == null)
                {
                    return NotFound(new { message = "User tidak ditemukan" });
                }

                // Verify current password
                var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password);

                if (!isCurrentPasswordValid)
                {
                    return BadRequest(new { message = "Password lama tidak sesuai" });
                }

                // Hash new password
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Password berhasil diubah" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password");
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengubah password" });
            }
        }
    }
}
