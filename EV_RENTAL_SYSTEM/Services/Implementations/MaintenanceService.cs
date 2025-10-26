using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class MaintenanceService : BaseService, IMaintenanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MaintenanceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MaintenanceService> logger) : base(mapper, logger)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MaintenanceListResponseDto> GetAllMaintenancesAsync()
        {
            try
            {
                var maintenances = await _unitOfWork.Maintenances.GetAllAsync();
                var maintenanceDtos = maintenances.Select(m => new MaintenanceDto
                {
                    MaintenanceId = m.MaintenanceId,
                    Description = m.Description,
                    Cost = m.Cost,
                    MaintenanceDate = m.MaintenanceDate,
                    Status = m.Status,
                    LicensePlateId = m.LicensePlateId,
                    PlateNumber = m.LicensePlate?.PlateNumber,
                    VehicleModel = m.LicensePlate?.Vehicle?.Model,
                    StationName = m.LicensePlate?.Station?.StationName
                }).ToList();

                return new MaintenanceListResponseDto
                {
                    Success = true,
                    Message = "Lấy danh sách bảo trì thành công",
                    Data = maintenanceDtos,
                    TotalCount = maintenanceDtos.Count,
                    PageNumber = 1,
                    PageSize = maintenanceDtos.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all maintenances: {Error}", ex.Message);
                return new MaintenanceListResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách bảo trì: {ex.Message}",
                    Data = new List<MaintenanceDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<MaintenanceResponseDto> GetMaintenanceByIdAsync(int maintenanceId)
        {
            try
            {
                var maintenance = await _unitOfWork.Maintenances.GetByIdAsync(maintenanceId);
                if (maintenance == null)
                {
                    return new MaintenanceResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy bảo trì"
                    };
                }

                var maintenanceDto = new MaintenanceDto
                {
                    MaintenanceId = maintenance.MaintenanceId,
                    Description = maintenance.Description,
                    Cost = maintenance.Cost,
                    MaintenanceDate = maintenance.MaintenanceDate,
                    Status = maintenance.Status,
                    LicensePlateId = maintenance.LicensePlateId,
                    PlateNumber = maintenance.LicensePlate?.PlateNumber,
                    VehicleModel = maintenance.LicensePlate?.Vehicle?.Model,
                    StationName = maintenance.LicensePlate?.Station?.StationName
                };

                return new MaintenanceResponseDto
                {
                    Success = true,
                    Message = "Lấy thông tin bảo trì thành công",
                    Data = maintenanceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenance by ID {MaintenanceId}: {Error}", maintenanceId, ex.Message);
                return new MaintenanceResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi lấy thông tin bảo trì: {ex.Message}"
                };
            }
        }

        public async Task<MaintenanceResponseDto> CreateMaintenanceAsync(CreateMaintenanceDto createDto)
        {
            try
            {
                // Kiểm tra biển số xe có tồn tại không
                var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(createDto.LicensePlateId);
                if (licensePlate == null)
                {
                    return new MaintenanceResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy biển số xe với ID này"
                    };
                }

                // Tạo bảo trì mới
                var maintenance = new Maintenance
                {
                    Description = createDto.Description,
                    Cost = createDto.Cost,
                    MaintenanceDate = createDto.MaintenanceDate ?? DateTime.Now,
                    Status = createDto.Status ?? "Scheduled",
                    LicensePlateId = createDto.LicensePlateId
                };

                await _unitOfWork.Maintenances.AddAsync(maintenance);

                // Cập nhật trạng thái biển số xe thành "Maintenance"
                licensePlate.Status = "Maintenance";
                _unitOfWork.LicensePlates.Update(licensePlate);

                await _unitOfWork.SaveChangesAsync();

                // Reload với đầy đủ thông tin
                var createdMaintenance = await _unitOfWork.Maintenances.GetByIdAsync(maintenance.MaintenanceId);
                var maintenanceDto = new MaintenanceDto
                {
                    MaintenanceId = createdMaintenance!.MaintenanceId,
                    Description = createdMaintenance.Description,
                    Cost = createdMaintenance.Cost,
                    MaintenanceDate = createdMaintenance.MaintenanceDate,
                    Status = createdMaintenance.Status,
                    LicensePlateId = createdMaintenance.LicensePlateId,
                    PlateNumber = createdMaintenance.LicensePlate?.PlateNumber,
                    VehicleModel = createdMaintenance.LicensePlate?.Vehicle?.Model,
                    StationName = createdMaintenance.LicensePlate?.Station?.StationName
                };

                _logger.LogInformation("Created maintenance {MaintenanceId} for license plate {LicensePlateId}", 
                    maintenance.MaintenanceId, createDto.LicensePlateId);

                return new MaintenanceResponseDto
                {
                    Success = true,
                    Message = "Tạo bảo trì thành công",
                    Data = maintenanceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating maintenance: {Error}", ex.Message);
                return new MaintenanceResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi tạo bảo trì: {ex.Message}"
                };
            }
        }

        public async Task<MaintenanceResponseDto> UpdateMaintenanceAsync(int maintenanceId, UpdateMaintenanceDto updateDto)
        {
            try
            {
                var maintenance = await _unitOfWork.Maintenances.GetByIdAsync(maintenanceId);
                if (maintenance == null)
                {
                    return new MaintenanceResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy bảo trì"
                    };
                }

                var wasCompleted = maintenance.Status == "Completed";

                // Cập nhật thông tin
                if (updateDto.Description != null)
                    maintenance.Description = updateDto.Description;
                if (updateDto.Cost.HasValue)
                    maintenance.Cost = updateDto.Cost.Value;
                if (updateDto.MaintenanceDate.HasValue)
                    maintenance.MaintenanceDate = updateDto.MaintenanceDate.Value;
                if (updateDto.Status != null)
                    maintenance.Status = updateDto.Status;

                _unitOfWork.Maintenances.Update(maintenance);

                // Nếu trạng thái chuyển sang "Completed", auto update battery 100% và status Available
                if (updateDto.Status == "Completed" && !wasCompleted)
                {
                    var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(maintenance.LicensePlateId);
                    if (licensePlate != null)
                    {
                        licensePlate.Status = "Available";
                        _unitOfWork.LicensePlates.Update(licensePlate);

                        // Update vehicle battery to 100%
                        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(licensePlate.VehicleId);
                        if (vehicle != null)
                        {
                            vehicle.Battery = 100;
                            await _unitOfWork.Vehicles.UpdateAsync(vehicle);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // Reload với đầy đủ thông tin
                var updatedMaintenance = await _unitOfWork.Maintenances.GetByIdAsync(maintenanceId);
                var maintenanceDto = new MaintenanceDto
                {
                    MaintenanceId = updatedMaintenance!.MaintenanceId,
                    Description = updatedMaintenance.Description,
                    Cost = updatedMaintenance.Cost,
                    MaintenanceDate = updatedMaintenance.MaintenanceDate,
                    Status = updatedMaintenance.Status,
                    LicensePlateId = updatedMaintenance.LicensePlateId,
                    PlateNumber = updatedMaintenance.LicensePlate?.PlateNumber,
                    VehicleModel = updatedMaintenance.LicensePlate?.Vehicle?.Model,
                    StationName = updatedMaintenance.LicensePlate?.Station?.StationName
                };

                return new MaintenanceResponseDto
                {
                    Success = true,
                    Message = "Cập nhật bảo trì thành công",
                    Data = maintenanceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating maintenance {MaintenanceId}: {Error}", maintenanceId, ex.Message);
                return new MaintenanceResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi cập nhật bảo trì: {ex.Message}"
                };
            }
        }

        public async Task<MaintenanceResponseDto> DeleteMaintenanceAsync(int maintenanceId)
        {
            try
            {
                var maintenance = await _unitOfWork.Maintenances.GetByIdAsync(maintenanceId);
                if (maintenance == null)
                {
                    return new MaintenanceResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy bảo trì"
                    };
                }

                // Nếu bảo trì đã hoàn thành, không cho phép xóa
                if (maintenance.Status == "Completed")
                {
                    return new MaintenanceResponseDto
                    {
                        Success = false,
                        Message = "Không thể xóa bảo trì đã hoàn thành"
                    };
                }

                // Cập nhật trạng thái biển số xe về "Available" nếu bảo trì đang Scheduled hoặc In Progress
                var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(maintenance.LicensePlateId);
                if (licensePlate != null && (maintenance.Status == "Scheduled" || maintenance.Status == "In Progress"))
                {
                    licensePlate.Status = "Available";
                    _unitOfWork.LicensePlates.Update(licensePlate);
                }

                _unitOfWork.Maintenances.Remove(maintenance);
                await _unitOfWork.SaveChangesAsync();

                return new MaintenanceResponseDto
                {
                    Success = true,
                    Message = "Xóa bảo trì thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting maintenance {MaintenanceId}: {Error}", maintenanceId, ex.Message);
                return new MaintenanceResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi xóa bảo trì: {ex.Message}"
                };
            }
        }

        public async Task<MaintenanceListResponseDto> SearchMaintenancesAsync(MaintenanceSearchDto searchDto)
        {
            try
            {
                var allMaintenances = await _unitOfWork.Maintenances.GetAllAsync();
                var filteredMaintenances = allMaintenances.AsQueryable();

                if (searchDto.LicensePlateId.HasValue)
                    filteredMaintenances = filteredMaintenances.Where(m => m.LicensePlateId == searchDto.LicensePlateId.Value);

                if (!string.IsNullOrEmpty(searchDto.Status))
                    filteredMaintenances = filteredMaintenances.Where(m => m.Status == searchDto.Status);

                if (searchDto.StartDate.HasValue)
                    filteredMaintenances = filteredMaintenances.Where(m => m.MaintenanceDate >= searchDto.StartDate.Value);

                if (searchDto.EndDate.HasValue)
                    filteredMaintenances = filteredMaintenances.Where(m => m.MaintenanceDate <= searchDto.EndDate.Value);

                var maintenanceList = filteredMaintenances.ToList();
                var maintenanceDtos = maintenanceList.Select(m => new MaintenanceDto
                {
                    MaintenanceId = m.MaintenanceId,
                    Description = m.Description,
                    Cost = m.Cost,
                    MaintenanceDate = m.MaintenanceDate,
                    Status = m.Status,
                    LicensePlateId = m.LicensePlateId,
                    PlateNumber = m.LicensePlate?.PlateNumber,
                    VehicleModel = m.LicensePlate?.Vehicle?.Model,
                    StationName = m.LicensePlate?.Station?.StationName
                }).ToList();

                return new MaintenanceListResponseDto
                {
                    Success = true,
                    Message = "Tìm kiếm bảo trì thành công",
                    Data = maintenanceDtos,
                    TotalCount = maintenanceDtos.Count,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = (int)Math.Ceiling((double)maintenanceDtos.Count / searchDto.PageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching maintenances: {Error}", ex.Message);
                return new MaintenanceListResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi tìm kiếm bảo trì: {ex.Message}",
                    Data = new List<MaintenanceDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<MaintenanceListResponseDto> GetMaintenancesByLicensePlateIdAsync(int licensePlateId)
        {
            try
            {
                var maintenances = await _unitOfWork.Maintenances.GetMaintenancesByLicensePlateIdAsync(licensePlateId);
                var maintenanceDtos = maintenances.Select(m => new MaintenanceDto
                {
                    MaintenanceId = m.MaintenanceId,
                    Description = m.Description,
                    Cost = m.Cost,
                    MaintenanceDate = m.MaintenanceDate,
                    Status = m.Status,
                    LicensePlateId = m.LicensePlateId,
                    PlateNumber = m.LicensePlate?.PlateNumber,
                    VehicleModel = m.LicensePlate?.Vehicle?.Model,
                    StationName = m.LicensePlate?.Station?.StationName
                }).ToList();

                return new MaintenanceListResponseDto
                {
                    Success = true,
                    Message = "Lấy danh sách bảo trì theo biển số thành công",
                    Data = maintenanceDtos,
                    TotalCount = maintenanceDtos.Count,
                    PageNumber = 1,
                    PageSize = maintenanceDtos.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenances for license plate {LicensePlateId}: {Error}", licensePlateId, ex.Message);
                return new MaintenanceListResponseDto
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách bảo trì: {ex.Message}",
                    Data = new List<MaintenanceDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }
    }
}

