using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    /// <summary>
    /// DTO cho thống kê doanh thu tổng quan
    /// </summary>
    public class RevenueOverviewDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public decimal ThisYearRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int ActiveOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    /// <summary>
    /// DTO cho thống kê doanh thu
    /// </summary>
    public class RevenueDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public decimal ThisYearRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int ActiveOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    /// <summary>
    /// DTO cho doanh thu theo ngày
    /// </summary>
    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    /// <summary>
    /// DTO cho doanh thu theo tháng
    /// </summary>
    public class MonthlyRevenueDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    /// <summary>
    /// DTO cho doanh thu theo trạm
    /// </summary>
    public class StationRevenueDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    /// <summary>
    /// DTO cho doanh thu theo loại xe
    /// </summary>
    public class VehicleTypeRevenueDto
    {
        public string VehicleType { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    /// <summary>
    /// DTO cho top xe được thuê nhiều nhất
    /// </summary>
    public class TopVehicleDto
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public int RentalCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRevenue { get; set; }
    }

    /// <summary>
    /// DTO cho request lọc doanh thu
    /// </summary>
    public class RevenueFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StationId { get; set; }
        public string? VehicleType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// DTO cho báo cáo doanh thu chi tiết
    /// </summary>
    public class DetailedRevenueReportDto
    {
        public RevenueOverviewDto Overview { get; set; } = new RevenueOverviewDto();
        public List<DailyRevenueDto> DailyRevenue { get; set; } = new List<DailyRevenueDto>();
        public List<StationRevenueDto> StationRevenue { get; set; } = new List<StationRevenueDto>();
        public List<VehicleTypeRevenueDto> VehicleTypeRevenue { get; set; } = new List<VehicleTypeRevenueDto>();
        public List<TopVehicleDto> TopVehicles { get; set; } = new List<TopVehicleDto>();
    }

    /// <summary>
    /// DTO cho response API
    /// </summary>
    public class RevenueResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public RevenueDto? Data { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách doanh thu
    /// </summary>
    public class RevenueListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<DailyRevenueDto> Data { get; set; } = new List<DailyRevenueDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
