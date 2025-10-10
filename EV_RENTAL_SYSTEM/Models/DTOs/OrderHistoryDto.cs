using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    /// <summary>
    /// DTO cho lịch sử chuyến của user
    /// </summary>
    public class OrderHistoryDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Status { get; set; }
        
        // Thông tin xe
        public string? VehicleModel { get; set; }
        public string? VehicleBrand { get; set; }
        public string? VehicleType { get; set; }
        public int? LicensePlateId { get; set; }
        
        // Thông tin trạm
        public string? StationName { get; set; }
        public string? StationAddress { get; set; }
        
        // Thông tin bằng lái được sử dụng
        public string? LicenseNumber { get; set; }
        public string? LicenseType { get; set; }
    }

    /// <summary>
    /// DTO cho response danh sách lịch sử chuyến
    /// </summary>
    public class OrderHistoryListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<OrderHistoryDto> Data { get; set; } = new List<OrderHistoryDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}




