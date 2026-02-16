using System.ComponentModel.DataAnnotations;

namespace BorrowdungAPI.Models.DTOs.User
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Password lama harus diisi")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password baru harus diisi")]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konfirmasi password harus diisi")]
        [Compare("NewPassword", ErrorMessage = "Password baru dan konfirmasi tidak cocok")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
