using System.ComponentModel.DataAnnotations;

namespace BorrowdungAPI.Models.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username harus diisi")]
        [MinLength(3, ErrorMessage = "Username minimal 3 karakter")]
        [MaxLength(50, ErrorMessage = "Username maksimal 50 karakter")]
        [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "Username hanya boleh mengandung huruf, angka, dan underscore")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email harus diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [MaxLength(100, ErrorMessage = "Email maksimal 100 karakter")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password harus diisi")]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter")]
        [MaxLength(100, ErrorMessage = "Password maksimal 100 karakter")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konfirmasi password harus diisi")]
        [Compare("Password", ErrorMessage = "Password dan konfirmasi password tidak cocok")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama lengkap harus diisi")]
        [MaxLength(100, ErrorMessage = "Nama lengkap maksimal 100 karakter")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Nomor telepon maksimal 50 karakter")]
        public string? PhoneNumber { get; set; }
    }
}
