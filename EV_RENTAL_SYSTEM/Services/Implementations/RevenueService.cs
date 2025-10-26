using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class RevenueService : IRevenueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RevenueService> _logger;

        public RevenueService(IUnitOfWork unitOfWork, ILogger<RevenueService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResponse<RevenueOverviewDto>> GetRevenueOverviewAsync()
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);
                var thisYear = new DateTime(today.Year, 1, 1);

                // Lấy tất cả orders
                var allOrders = await _unitOfWork.Orders.GetAllAsync();
                var orders = allOrders.ToList();

                // Tính tổng doanh thu bao gồm extra fee
                decimal totalRevenue = 0;
                decimal todayRevenue = 0;
                decimal thisMonthRevenue = 0;
                decimal thisYearRevenue = 0;

                foreach (var order in orders)
                {
                    var orderAmount = order.TotalAmount.GetValueOrDefault(0);
                    var contract = await _unitOfWork.Contracts.GetContractByOrderIdAsync(order.OrderId);
                    var extraFee = contract?.ExtraFee.GetValueOrDefault(0) ?? 0;
                    var totalOrderRevenue = orderAmount + extraFee;

                    totalRevenue += totalOrderRevenue;
                    if (order.OrderDate.Date == today)
                        todayRevenue += totalOrderRevenue;
                    if (order.OrderDate >= thisMonth)
                        thisMonthRevenue += totalOrderRevenue;
                    if (order.OrderDate >= thisYear)
                        thisYearRevenue += totalOrderRevenue;
                }

                // Thống kê orders
                var totalOrders = orders.Count;
                var completedOrders = orders.Count(o => o.Status == "Completed");
                var activeOrders = orders.Count(o => o.Status == "Active" || o.Status == "Rented");
                var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

                var overview = new RevenueOverviewDto
                {
                    TotalRevenue = totalRevenue,
                    TodayRevenue = todayRevenue,
                    ThisMonthRevenue = thisMonthRevenue,
                    ThisYearRevenue = thisYearRevenue,
                    TotalOrders = totalOrders,
                    CompletedOrders = completedOrders,
                    ActiveOrders = activeOrders,
                    AverageOrderValue = averageOrderValue
                };

                return ServiceResponse<RevenueOverviewDto>.SuccessResult(overview, "Revenue overview retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue overview");
                return ServiceResponse<RevenueOverviewDto>.ErrorResult($"Error getting revenue overview: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<DailyRevenueDto>>> GetDailyRevenueAsync(RevenueFilterDto filter)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var ordersList = orders.ToList();

                // Áp dụng filter
                if (filter.StartDate.HasValue)
                    ordersList = ordersList.Where(o => o.OrderDate >= filter.StartDate.Value).ToList();
                if (filter.EndDate.HasValue)
                    ordersList = ordersList.Where(o => o.OrderDate <= filter.EndDate.Value).ToList();

                // Nhóm theo ngày
                var dailyRevenue = ordersList
                    .Where(o => o.TotalAmount.HasValue)
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new DailyRevenueDto
                    {
                        Date = g.Key,
                        Revenue = g.Sum(o => o.TotalAmount.GetValueOrDefault(0)),
                        OrderCount = g.Count(),
                        AverageOrderValue = g.Average(o => o.TotalAmount.GetValueOrDefault(0))
                    })
                    .OrderByDescending(d => d.Date)
                    .ToList();

                return ServiceResponse<List<DailyRevenueDto>>.SuccessResult(dailyRevenue, "Daily revenue retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily revenue");
                return ServiceResponse<List<DailyRevenueDto>>.ErrorResult($"Error getting daily revenue: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<MonthlyRevenueDto>>> GetMonthlyRevenueAsync(int year)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var ordersList = orders.ToList();

                // Lọc theo năm
                ordersList = ordersList.Where(o => o.OrderDate.Year == year).ToList();

                // Nhóm theo tháng
                var monthlyRevenue = ordersList
                    .Where(o => o.TotalAmount.HasValue)
                    .GroupBy(o => o.OrderDate.Month)
                    .Select(g => new MonthlyRevenueDto
                    {
                        Year = year,
                        Month = g.Key,
                        MonthName = new DateTime(year, g.Key, 1).ToString("MMMM"),
                        Revenue = g.Sum(o => o.TotalAmount.GetValueOrDefault(0)),
                        OrderCount = g.Count(),
                        AverageOrderValue = g.Average(o => o.TotalAmount.GetValueOrDefault(0))
                    })
                    .OrderBy(m => m.Month)
                    .ToList();

                return ServiceResponse<List<MonthlyRevenueDto>>.SuccessResult(monthlyRevenue, "Monthly revenue retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly revenue");
                return ServiceResponse<List<MonthlyRevenueDto>>.ErrorResult($"Error getting monthly revenue: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<StationRevenueDto>>> GetRevenueByStationAsync(RevenueFilterDto filter)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var ordersList = orders.ToList();

                // Áp dụng filter
                if (filter.StartDate.HasValue)
                    ordersList = ordersList.Where(o => o.OrderDate >= filter.StartDate.Value).ToList();
                if (filter.EndDate.HasValue)
                    ordersList = ordersList.Where(o => o.OrderDate <= filter.EndDate.Value).ToList();

                // Lấy tất cả stations
                var stations = await _unitOfWork.Stations.GetAllAsync();
                var stationRevenue = new List<StationRevenueDto>();

                foreach (var station in stations)
                {
                    var stationOrders = ordersList.Where(o => 
                        o.OrderLicensePlates.Any(olp => olp.LicensePlate.StationId == station.StationId)).ToList();
                    
                    var revenue = stationOrders.Where(o => o.TotalAmount.HasValue).Sum(o => o.TotalAmount.GetValueOrDefault(0));
                    var orderCount = stationOrders.Count;
                    var averageOrderValue = orderCount > 0 ? revenue / orderCount : 0;

                    stationRevenue.Add(new StationRevenueDto
                    {
                        StationId = station.StationId,
                        StationName = station.StationName,
                        Province = station.Province,
                        Revenue = revenue,
                        OrderCount = orderCount,
                        AverageOrderValue = averageOrderValue
                    });
                }

                return ServiceResponse<List<StationRevenueDto>>.SuccessResult(stationRevenue, "Station revenue retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting station revenue");
                return ServiceResponse<List<StationRevenueDto>>.ErrorResult($"Error getting station revenue: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<VehicleTypeRevenueDto>>> GetRevenueByVehicleTypeAsync(RevenueFilterDto filter)
        {
            try
            {
                // VehicleType has been removed; return empty breakdown
                return ServiceResponse<List<VehicleTypeRevenueDto>>.SuccessResult(new List<VehicleTypeRevenueDto>(), "Vehicle type revenue not applicable.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vehicle type revenue");
                return ServiceResponse<List<VehicleTypeRevenueDto>>.ErrorResult($"Error getting vehicle type revenue: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<TopVehicleDto>>> GetTopRentedVehiclesAsync(RevenueFilterDto filter, int top = 5)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                var ordersList = orders.ToList();

                // Áp dụng filter
                if (filter.StartDate.HasValue)
                    ordersList = ordersList.Where(o => o.OrderDate >= filter.StartDate.Value).ToList();
                if (filter.EndDate.HasValue)
                    ordersList = ordersList.Where(o => o.OrderDate <= filter.EndDate.Value).ToList();

                // Lấy tất cả vehicles
                var vehicles = await _unitOfWork.Vehicles.GetAllAsync();
                var topVehicles = new List<TopVehicleDto>();

                foreach (var vehicle in vehicles)
                {
                    var vehicleOrders = ordersList.Where(o => 
                        o.OrderLicensePlates.Any(olp => olp.LicensePlate.VehicleId == vehicle.VehicleId)).ToList();
                    
                    var rentalCount = vehicleOrders.Count;
                    var totalRevenue = vehicleOrders.Where(o => o.TotalAmount.HasValue).Sum(o => o.TotalAmount.GetValueOrDefault(0));
                    var averageRevenue = rentalCount > 0 ? totalRevenue / rentalCount : 0;

                    topVehicles.Add(new TopVehicleDto
                    {
                        VehicleId = vehicle.VehicleId,
                        Model = vehicle.Model,
                        BrandName = vehicle.Brand?.BrandName ?? "Unknown",
                        RentalCount = rentalCount,
                        TotalRevenue = totalRevenue,
                        AverageRevenue = averageRevenue
                    });
                }

                // Sắp xếp theo số lần thuê và lấy top
                var result = topVehicles
                    .OrderByDescending(v => v.RentalCount)
                    .ThenByDescending(v => v.TotalRevenue)
                    .Take(top)
                    .ToList();

                return ServiceResponse<List<TopVehicleDto>>.SuccessResult(result, "Top rented vehicles retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top rented vehicles");
                return ServiceResponse<List<TopVehicleDto>>.ErrorResult($"Error getting top rented vehicles: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<DetailedRevenueReportDto>> GetDetailedRevenueReportAsync(RevenueFilterDto filter)
        {
            try
            {
                // Lấy tất cả dữ liệu
                var overviewTask = GetRevenueOverviewAsync();
                var dailyTask = GetDailyRevenueAsync(filter);
                var stationTask = GetRevenueByStationAsync(filter);
                var vehicleTypeTask = GetRevenueByVehicleTypeAsync(filter);
                var topVehiclesTask = GetTopRentedVehiclesAsync(filter, 5);

                await Task.WhenAll(overviewTask, dailyTask, stationTask, vehicleTypeTask, topVehiclesTask);

                var report = new DetailedRevenueReportDto
                {
                    Overview = overviewTask.Result.Data ?? new RevenueOverviewDto(),
                    DailyRevenue = dailyTask.Result.Data ?? new List<DailyRevenueDto>(),
                    StationRevenue = stationTask.Result.Data ?? new List<StationRevenueDto>(),
                    VehicleTypeRevenue = vehicleTypeTask.Result.Data ?? new List<VehicleTypeRevenueDto>(),
                    TopVehicles = topVehiclesTask.Result.Data ?? new List<TopVehicleDto>()
                };

                return ServiceResponse<DetailedRevenueReportDto>.SuccessResult(report, "Detailed revenue report retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting detailed revenue report");
                return ServiceResponse<DetailedRevenueReportDto>.ErrorResult($"Error getting detailed revenue report: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> ExportRevenueReportToExcelAsync(RevenueFilterDto filter)
        {
            try
            {
                // Mock implementation - trong thực tế sẽ tạo file Excel
                var fileName = $"RevenueReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var filePath = Path.Combine("wwwroot", "reports", fileName);

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                // Mock file creation
                await File.WriteAllTextAsync(filePath, "Mock Excel content");

                return ServiceResponse<string>.SuccessResult(filePath, "Revenue report exported successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting revenue report");
                return ServiceResponse<string>.ErrorResult($"Error exporting revenue report: {ex.Message}");
            }
        }
    }
}