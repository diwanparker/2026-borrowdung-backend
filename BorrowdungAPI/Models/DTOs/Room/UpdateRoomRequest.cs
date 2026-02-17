using System.ComponentModel.DataAnnotations;

namespace BorrowdungAPI.Models.DTOs.Room
{
    public class UpdateRoomRequest
    {
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? Name { get; set; }

        [MaxLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        public string? Location { get; set; }

        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        public int? Capacity { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string? Status { get; set; }
    }
}
