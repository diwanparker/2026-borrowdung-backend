using System.ComponentModel.DataAnnotations;
using BorrowdungAPI.Models.Enums;

namespace BorrowdungAPI.Models.DTOs.User
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Username harus diisi")]
        [MinLength(3, ErrorMessage = "Username minimal 3 karakter")]
        [MaxLength(50, ErrorMessage = "Username maksimal 50 karakter")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email harus diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [MaxLength(100, ErrorMessage = "Email maksimal 100 karakter")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password harus diisi")]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama lengkap harus diisi")]
        [MaxLength(100, ErrorMessage = "Nama lengkap maksimal 100 karakter")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Nomor telepon maksimal 50 karakter")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role harus diisi")]
        public UserRole Role { get; set; } = UserRole.User;
    }
}
