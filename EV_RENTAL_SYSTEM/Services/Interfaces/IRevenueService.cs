using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IRevenueService
    {
        /// <summary>
        /// Lấy tổng quan doanh thu
        /// </summary>
        /// <returns>Thống kê tổng quan doanh thu</returns>
        Task<ServiceResponse<RevenueOverviewDto>> GetRevenueOverviewAsync();

        /// <summary>
        /// Lấy doanh thu theo ngày
        /// </summary>
        /// <param name="filter">Bộ lọc doanh thu</param>
        /// <returns>Danh sách doanh thu theo ngày</returns>
        Task<ServiceResponse<List<DailyRevenueDto>>> GetDailyRevenueAsync(RevenueFilterDto filter);

        /// <summary>
        /// Lấy doanh thu theo tháng
        /// </summary>
        /// <param name="year">Năm</param>
        /// <returns>Danh sách doanh thu theo tháng</returns>
        Task<ServiceResponse<List<MonthlyRevenueDto>>> GetMonthlyRevenueAsync(int year);

        /// <summary>
        /// Lấy doanh thu theo trạm
        /// </summary>
        /// <param name="filter">Bộ lọc doanh thu</param>
        /// <returns>Danh sách doanh thu theo trạm</returns>
        Task<ServiceResponse<List<StationRevenueDto>>> GetRevenueByStationAsync(RevenueFilterDto filter);

        /// <summary>
        /// Lấy doanh thu theo loại xe
        /// </summary>
        /// <param name="filter">Bộ lọc doanh thu</param>
        /// <returns>Danh sách doanh thu theo loại xe</returns>
        Task<ServiceResponse<List<VehicleTypeRevenueDto>>> GetRevenueByVehicleTypeAsync(RevenueFilterDto filter);

        /// <summary>
        /// Lấy top xe được thuê nhiều nhất
        /// </summary>
        /// <param name="filter">Bộ lọc doanh thu</param>
        /// <param name="top">Số lượng top</param>
        /// <returns>Danh sách top xe</returns>
        Task<ServiceResponse<List<TopVehicleDto>>> GetTopRentedVehiclesAsync(RevenueFilterDto filter, int top = 5);

        /// <summary>
        /// Lấy báo cáo doanh thu chi tiết
        /// </summary>
        /// <param name="filter">Bộ lọc doanh thu</param>
        /// <returns>Báo cáo doanh thu chi tiết</returns>
        Task<ServiceResponse<DetailedRevenueReportDto>> GetDetailedRevenueReportAsync(RevenueFilterDto filter);

        /// <summary>
        /// Export báo cáo doanh thu ra Excel
        /// </summary>
        /// <param name="filter">Bộ lọc doanh thu</param>
        /// <returns>Đường dẫn file Excel</returns>
        Task<ServiceResponse<string>> ExportRevenueReportToExcelAsync(RevenueFilterDto filter);
    }
}