using System.ComponentModel.DataAnnotations;

namespace BorrowdungAPI.Models.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username atau email harus diisi")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password harus diisi")]
        public string Password { get; set; } = string.Empty;
    }
}
