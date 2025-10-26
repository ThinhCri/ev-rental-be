using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    /// <summary>
    /// DTO cho request tạo bảo trì xe
    /// </summary>
    public class CreateMaintenanceDto
    {
        [Required(ErrorMessage = "Mô tả bảo trì là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Mô tả không được quá 255 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Chi phí phải lớn hơn hoặc bằng 0")]
        public decimal? Cost { get; set; }

        public DateTime? MaintenanceDate { get; set; }

        [Required(ErrorMessage = "ID biển số xe là bắt buộc")]
        public int LicensePlateId { get; set; }

        [MaxLength(50, ErrorMessage = "Trạng thái không được quá 50 ký tự")]
        public string? Status { get; set; } = "Scheduled"; // Scheduled, In Progress, Completed, Cancelled
    }

    /// <summary>
    /// DTO cho request cập nhật bảo trì
    /// </summary>
    public class UpdateMaintenanceDto
    {
        [MaxLength(255, ErrorMessage = "Mô tả không được quá 255 ký tự")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chi phí phải lớn hơn hoặc bằng 0")]
        public decimal? Cost { get; set; }

        public DateTime? MaintenanceDate { get; set; }

        [MaxLength(50, ErrorMessage = "Trạng thái không được quá 50 ký tự")]
        public string? Status { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin bảo trì (trả về cho client)
    /// </summary>
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
    }

    /// <summary>
    /// DTO cho response API bảo trì
    /// </summary>
    public class MaintenanceResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public MaintenanceDto? Data { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách bảo trì
    /// </summary>
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

    /// <summary>
    /// DTO cho request tìm kiếm bảo trì
    /// </summary>
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

