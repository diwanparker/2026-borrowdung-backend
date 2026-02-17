using System.ComponentModel.DataAnnotations;

namespace BorrowdungAPI.Models.DTOs.Booking
{
    public class CreateBookingRequest
    {
        [Required(ErrorMessage = "RoomId is required")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Booker name is required")]
        [MaxLength(100, ErrorMessage = "Booker name cannot exceed 100 characters")]
        public string BookerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string BookerEmail { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Phone number cannot exceed 50 characters")]
        public string? BookerPhone { get; set; }

        [Required(ErrorMessage = "Purpose is required")]
        [MaxLength(500, ErrorMessage = "Purpose cannot exceed 500 characters")]
        public string Purpose { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }
    }
}
