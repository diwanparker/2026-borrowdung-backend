using System.ComponentModel.DataAnnotations;

namespace BorrowdungAPI.Models.DTOs.User
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Email harus diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [MaxLength(100, ErrorMessage = "Email maksimal 100 karakter")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama lengkap harus diisi")]
        [MaxLength(100, ErrorMessage = "Nama lengkap maksimal 100 karakter")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Nomor telepon maksimal 50 karakter")]
        public string? PhoneNumber { get; set; }
    }
}
