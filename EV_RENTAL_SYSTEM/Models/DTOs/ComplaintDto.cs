using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    public class CreateComplaintDto
    {
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(255, ErrorMessage = "Description must not exceed 255 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }
    }

    public class UpdateComplaintStatusDto
    {
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(50, ErrorMessage = "Status must not exceed 50 characters")]
        public string Status { get; set; } = string.Empty;
    }

    public class ComplaintDto
    {
        public int ComplaintId { get; set; }
        public DateTime ComplaintDate { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public DateTime? OrderStartTime { get; set; }
        public DateTime? OrderEndTime { get; set; }
        public List<ComplaintVehicleDto> Vehicles { get; set; } = new List<ComplaintVehicleDto>();
    }

    public class ComplaintVehicleDto
    {
        public int VehicleId { get; set; }
        public string? Model { get; set; }
        public string? BrandName { get; set; }
        public string? VehicleImage { get; set; }
        public string? PlateNumber { get; set; }
        public int? LicensePlateId { get; set; }
    }

    public class ComplaintResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ComplaintDto? Data { get; set; }
    }

    public class ComplaintListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ComplaintDto> Data { get; set; } = new List<ComplaintDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}

