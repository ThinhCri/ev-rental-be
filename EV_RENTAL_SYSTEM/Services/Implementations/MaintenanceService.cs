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
                    Message = "Maintenances retrieved successfully",
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
                    Message = $"Error retrieving maintenance list: {ex.Message}",
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
                        Message = "Maintenance not found"
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
                    Message = "Maintenance information retrieved successfully",
                    Data = maintenanceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenance by ID {MaintenanceId}: {Error}", maintenanceId, ex.Message);
                return new MaintenanceResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving maintenance information: {ex.Message}"
                };
            }
        }

        public async Task<MaintenanceResponseDto> CreateMaintenanceAsync(CreateMaintenanceDto createDto)
        {
            try
            {
                var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(createDto.LicensePlateId);
                if (licensePlate == null)
                {
                    return new MaintenanceResponseDto
                    {
                        Success = false,
                        Message = "License plate not found with this ID"
                    };
                }

                var maintenance = new Maintenance
                {
                    Description = createDto.Description,
                    Cost = createDto.Cost,
                    MaintenanceDate = createDto.MaintenanceDate ?? DateTime.Now,
                    Status = createDto.Status ?? "Scheduled",
                    LicensePlateId = createDto.LicensePlateId
                };

                await _unitOfWork.Maintenances.AddAsync(maintenance);

                licensePlate.Status = "Maintenance";
                _unitOfWork.LicensePlates.Update(licensePlate);

                await _unitOfWork.SaveChangesAsync();

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
                    Message = "Maintenance created successfully",
                    Data = maintenanceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating maintenance: {Error}", ex.Message);
                return new MaintenanceResponseDto
                {
                    Success = false,
                    Message = $"Error creating maintenance: {ex.Message}"
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
                        Message = "Maintenance not found"
                    };
                }

                var wasCompleted = maintenance.Status == "Completed";

                if (updateDto.Description != null)
                    maintenance.Description = updateDto.Description;
                if (updateDto.Cost.HasValue)
                    maintenance.Cost = updateDto.Cost.Value;
                if (updateDto.MaintenanceDate.HasValue)
                    maintenance.MaintenanceDate = updateDto.MaintenanceDate.Value;
                if (updateDto.Status != null)
                    maintenance.Status = updateDto.Status;

                _unitOfWork.Maintenances.Update(maintenance);

                if (updateDto.Status == "Completed" && !wasCompleted)
                {
                    var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(maintenance.LicensePlateId);
                    if (licensePlate != null)
                    {
                        licensePlate.Status = "Available";
                        _unitOfWork.LicensePlates.Update(licensePlate);

                        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(licensePlate.VehicleId);
                        if (vehicle != null)
                        {
                            vehicle.Battery = 100;
                            await _unitOfWork.Vehicles.UpdateAsync(vehicle);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();

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
                    Message = "Maintenance updated successfully",
                    Data = maintenanceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating maintenance {MaintenanceId}: {Error}", maintenanceId, ex.Message);
                return new MaintenanceResponseDto
                {
                    Success = false,
                    Message = $"Error updating maintenance: {ex.Message}"
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
                        Message = "Maintenance not found"
                    };
                }

                if (maintenance.Status == "Completed")
                {
                    return new MaintenanceResponseDto
                    {
                        Success = false,
                        Message = "Cannot delete completed maintenance"
                    };
                }

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
                    Message = "Maintenance deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting maintenance {MaintenanceId}: {Error}", maintenanceId, ex.Message);
                return new MaintenanceResponseDto
                {
                    Success = false,
                    Message = $"Error deleting maintenance: {ex.Message}"
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
                    Message = "Maintenance search completed successfully",
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
                    Message = $"Error searching maintenance: {ex.Message}",
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
                    Message = "Maintenances by license plate retrieved successfully",
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
                    Message = $"Error retrieving maintenance list: {ex.Message}",
                    Data = new List<MaintenanceDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<MaintenanceListResponseDto> GetMaintenancesByStationIdAsync(int stationId)
        {
            try
            {
                var allMaintenances = await _unitOfWork.Maintenances.GetAllAsync();
                
                _logger.LogInformation("Total maintenances: {Count}, StationId: {StationId}", allMaintenances.Count(), stationId);
                
                var maintenancesForStation = new List<Maintenance>();
                foreach (var maintenance in allMaintenances)
                {
                    var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(maintenance.LicensePlateId);
                    if (licensePlate != null && licensePlate.StationId == stationId)
                    {
                        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(licensePlate.VehicleId);
                        if (vehicle != null)
                        {
                            var station = await _unitOfWork.Stations.GetByIdAsync(licensePlate.StationId);
                            var brand = await _unitOfWork.Brands.GetByIdAsync(vehicle.BrandId);
                            
                            maintenance.LicensePlate = licensePlate;
                            licensePlate.Vehicle = vehicle;
                            licensePlate.Station = station;
                            vehicle.Brand = brand;
                            
                            maintenancesForStation.Add(maintenance);
                            _logger.LogInformation("Found maintenance {MaintenanceId} for station {StationId}, LicensePlate: {PlateNumber}, Vehicle: {VehicleModel}", 
                                maintenance.MaintenanceId, stationId, licensePlate.PlateNumber, vehicle.Model);
                        }
                    }
                }

                var maintenanceDtos = maintenancesForStation.Select(m => new MaintenanceDto
                {
                    MaintenanceId = m.MaintenanceId,
                    Description = m.Description,
                    Cost = m.Cost,
                    MaintenanceDate = m.MaintenanceDate,
                    Status = m.Status,
                    LicensePlateId = m.LicensePlateId,
                    PlateNumber = m.LicensePlate?.PlateNumber,
                    VehicleModel = m.LicensePlate?.Vehicle?.Model,
                    StationName = m.LicensePlate?.Station?.StationName,
                    VehicleImage = m.LicensePlate?.Vehicle?.VehicleImage,
                    BrandName = m.LicensePlate?.Vehicle?.Brand?.BrandName,
                    VehicleId = m.LicensePlate?.VehicleId
                }).ToList();

                return new MaintenanceListResponseDto
                {
                    Success = true,
                    Message = "Maintenances by station retrieved successfully",
                    Data = maintenanceDtos,
                    TotalCount = maintenanceDtos.Count,
                    PageNumber = 1,
                    PageSize = maintenanceDtos.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintenances for station {StationId}: {Error}", stationId, ex.Message);
                return new MaintenanceListResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving maintenance list: {ex.Message}",
                    Data = new List<MaintenanceDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<MaintenanceResponseDto> CompleteMaintenanceAsync(int maintenanceId, decimal? cost = null)
        {
            try
            {
                var maintenance = await _unitOfWork.Maintenances.GetByIdAsync(maintenanceId);
                if (maintenance == null)
                {
                    return new MaintenanceResponseDto
                    {
                        Success = false,
                        Message = "Maintenance not found"
                    };
                }

                maintenance.Status = "COMPLETED";
                maintenance.Cost = cost;
                _unitOfWork.Maintenances.Update(maintenance);

                var licensePlate = await _unitOfWork.LicensePlates.GetByIdAsync(maintenance.LicensePlateId);
                if (licensePlate != null)
                {
                    var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(licensePlate.VehicleId);
                    if (vehicle != null)
                    {
                        vehicle.Battery = 100;
                        await _unitOfWork.Vehicles.UpdateAsync(vehicle);
                        
                        licensePlate.Status = "Available";
                        _unitOfWork.LicensePlates.Update(licensePlate);
                        
                        _logger.LogInformation("Completed maintenance {MaintenanceId}, set battery to 100%, vehicle {VehicleId} available", 
                            maintenanceId, vehicle.VehicleId);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                var maintenanceDto = new MaintenanceDto
                {
                    MaintenanceId = maintenance.MaintenanceId,
                    Description = maintenance.Description,
                    Cost = maintenance.Cost,
                    MaintenanceDate = maintenance.MaintenanceDate,
                    Status = maintenance.Status,
                    LicensePlateId = maintenance.LicensePlateId,
                    PlateNumber = licensePlate?.PlateNumber,
                    VehicleModel = licensePlate?.Vehicle?.Model,
                    StationName = licensePlate?.Station?.StationName
                };

                return new MaintenanceResponseDto
                {
                    Success = true,
                    Message = "Maintenance completed successfully. Vehicle battery set to 100%",
                    Data = maintenanceDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing maintenance {MaintenanceId}: {Error}", maintenanceId, ex.Message);
                return new MaintenanceResponseDto
                {
                    Success = false,
                    Message = $"Error completing maintenance: {ex.Message}"
                };
            }
        }
    }
}

