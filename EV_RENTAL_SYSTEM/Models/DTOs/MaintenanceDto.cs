using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    public class CreateMaintenanceDto
    {
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(255, ErrorMessage = "Description must not exceed 255 characters")]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be greater than or equal to 0")]
        public decimal? Cost { get; set; }

        public DateTime? MaintenanceDate { get; set; }

        [Required(ErrorMessage = "License plate ID is required")]
        public int LicensePlateId { get; set; }

        [MaxLength(50, ErrorMessage = "Status must not exceed 50 characters")]
        public string? Status { get; set; } = "Scheduled";
    }

    public class UpdateMaintenanceDto
    {
        [MaxLength(255, ErrorMessage = "Description must not exceed 255 characters")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be greater than or equal to 0")]
        public decimal? Cost { get; set; }

        public DateTime? MaintenanceDate { get; set; }

        [MaxLength(50, ErrorMessage = "Status must not exceed 50 characters")]
        public string? Status { get; set; }
    }

    public class MaintenanceDto
    {
        public int MaintenanceId { get; set; }
        public string? Description { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public string? Status { get; set; }
        public int LicensePlateId { get; set; }
        public string? PlateNumber { get; set; }
        public string? VehicleModel { get; set; }
        public string? StationName { get; set; }
        public string? VehicleImage { get; set; }
        public string? BrandName { get; set; }
        public int? VehicleId { get; set; }
    }

    public class MaintenanceResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public MaintenanceDto? Data { get; set; }
    }

    public class MaintenanceListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<MaintenanceDto> Data { get; set; } = new List<MaintenanceDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class MaintenanceSearchDto
    {
        public int? LicensePlateId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
