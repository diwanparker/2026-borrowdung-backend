using System.ComponentModel.DataAnnotations;

namespace BorrowdungAPI.Models.DTOs.Booking
{
    public class UpdateBookingRequest
    {
        public int? RoomId { get; set; }

        [MaxLength(100, ErrorMessage = "Booker name cannot exceed 100 characters")]
        public string? BookerName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? BookerEmail { get; set; }

        [MaxLength(50, ErrorMessage = "Phone number cannot exceed 50 characters")]
        public string? BookerPhone { get; set; }

        [MaxLength(500, ErrorMessage = "Purpose cannot exceed 500 characters")]
        public string? Purpose { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
