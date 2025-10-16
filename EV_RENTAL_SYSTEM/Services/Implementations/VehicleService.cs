using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class VehicleService : BaseService, IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ILicensePlateRepository _licensePlateRepository;
        private readonly IStationRepository _stationRepository;
        private readonly IMapper _mapper;
        private readonly ICloudService _cloudService;
        private readonly IUnitOfWork _unitOfWork;

        public VehicleService(
            IVehicleRepository vehicleRepository,
            IBrandRepository brandRepository,
            ILicensePlateRepository licensePlateRepository,
            IStationRepository stationRepository,
            IMapper mapper,
            ILogger<VehicleService> logger,
            ICloudService cloudService,
            IUnitOfWork unitOfWork) : base(logger)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _licensePlateRepository = licensePlateRepository;
            _stationRepository = stationRepository;
            _mapper = mapper;
            _cloudService = cloudService;
            _unitOfWork = unitOfWork;
        }


        public async Task<VehicleResponseDto> GetByIdAsync(int id)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Invalid vehicle ID"
                    };
                }

                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Vehicle not found"
                    };
                }

                var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
                vehicleDto.Status = vehicle.LicensePlates.FirstOrDefault()?.Status ?? "Available";
                vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(id);
                vehicleDto.AvailableLicensePlates = vehicle.LicensePlates.Count(lp => lp.Status == "Available");
                vehicleDto.LicensePlateNumbers = vehicle.LicensePlates.Select(lp => lp.PlateNumber).ToList();
                // Derive station fields from the first license plate (if any)
                var firstPlate = vehicle.LicensePlates.FirstOrDefault();
                if (firstPlate != null)
                {
                    vehicleDto.StationId = firstPlate.StationId;
                    vehicleDto.StationName = firstPlate.Station?.StationName;
                    vehicleDto.StationStreet = firstPlate.Station?.Street;
                }

                return new VehicleResponseDto
                {
                    Success = true,
                    Message = "Vehicle information retrieved successfully",
                    Data = vehicleDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle with ID: {VehicleId}", id);
                return new VehicleResponseDto
                {
                    Success = false,
                    Message = "Error retrieving vehicle information"
                };
            }
        }

        public async Task<VehicleListResponseDto> GetAllAsync()
        {
            try
            {
                var vehicles = await _vehicleRepository.GetAllAsync();
                var vehicleDtos = new List<VehicleDto>();

                foreach (var vehicle in vehicles)
                {
                    var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
                    vehicleDto.Status = vehicle.LicensePlates.FirstOrDefault()?.Status ?? "Available";
                    vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(vehicle.VehicleId);
                    vehicleDto.AvailableLicensePlates = vehicle.LicensePlates.Count(lp => lp.Status == "Available");
                    vehicleDto.LicensePlateNumbers = vehicle.LicensePlates.Select(lp => lp.PlateNumber).ToList();
                    var firstPlate = vehicle.LicensePlates.FirstOrDefault();
                    if (firstPlate != null)
                    {
                        vehicleDto.StationId = firstPlate.StationId;
                        vehicleDto.StationName = firstPlate.Station?.StationName;
                        vehicleDto.StationStreet = firstPlate.Station?.Street;
                    }
                    vehicleDtos.Add(vehicleDto);
                }

                return new VehicleListResponseDto
                {
                    Success = true,
                    Message = "Vehicles retrieved successfully",
                    Data = vehicleDtos,
                    TotalCount = vehicleDtos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all vehicles");
                return new VehicleListResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách xe."
                };
            }
        }

        public async Task<VehicleResponseDto> CreateAsync(CreateVehicleDto createDto)
        {
            try
            {
                // Kiểm tra brand có tồn tại không
                var brand = await _brandRepository.GetByIdAsync(createDto.BrandId);
                if (brand == null)
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Brand not found"
                    };
                }

                // Kiểm tra biển số xe đã tồn tại chưa
                var existingLicensePlate = await _licensePlateRepository.GetLicensePlateByNumberAsync(createDto.LicensePlateNumber);
                if (existingLicensePlate != null)
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Biển số xe này đã tồn tại trong hệ thống"
                    };
                }

                // Upload ảnh lên cloud nếu có
                string? vehicleImageUrl = null;
                if (createDto.VehicleImageFile != null)
                {
                    try
                    {
                        vehicleImageUrl = await _cloudService.UploadImageAsync(createDto.VehicleImageFile);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading vehicle image");
                        return new VehicleResponseDto
                        {
                            Success = false,
                            Message = "Lỗi khi upload ảnh xe. Vui lòng thử lại."
                        };
                    }
                }

                // Tạo vehicle entity
                var vehicle = _mapper.Map<Vehicle>(createDto);
                vehicle.Brand = brand;
                vehicle.VehicleImage = vehicleImageUrl;

                // Lưu vào database
                var createdVehicle = await _vehicleRepository.AddAsync(vehicle);

                // Tạo LicensePlate cho xe
                var licensePlate = new LicensePlate
                {
                    PlateNumber = createDto.LicensePlateNumber, // Sử dụng PlateNumber để lưu biển số
                    Status = "Available",
                    VehicleId = createdVehicle.VehicleId,
                    RegistrationDate = DateTime.Now,
                    StationId = createDto.StationId ?? 1 // Default station nếu không có
                };

                await _licensePlateRepository.AddAsync(licensePlate);

                // Cập nhật bộ đếm của trạm
                // Cập nhật bộ đếm của trạm dựa trên LicensePlate được gán
                if (licensePlate.StationId != 0)
                {
                    var station = await _unitOfWork.Stations.GetByIdAsync(licensePlate.StationId);
                    if (station != null)
                    {
                        station.TotalVehicle += 1;
                        // Xe mới tạo có ít nhất 1 biển số Available
                        station.AvailableVehicle += 1;
                        _unitOfWork.Stations.Update(station);
                    }
                }

                // Đảm bảo tất cả thay đổi được commit trước khi load lại
                await _unitOfWork.SaveChangesAsync();

                // Load lại vehicle với đầy đủ thông tin (bao gồm Station và LicensePlates)
                var vehicleWithDetails = await _vehicleRepository.GetByIdAsync(createdVehicle.VehicleId);

                var vehicleDto = _mapper.Map<VehicleDto>(vehicleWithDetails);
                vehicleDto.BrandName = brand.BrandName;
                vehicleDto.Status = "Available"; // Mới tạo nên có 1 biển số available
                vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(createdVehicle.VehicleId);
                vehicleDto.AvailableLicensePlates = 1; // Mới tạo nên có 1 biển số available
                vehicleDto.LicensePlateNumbers = new List<string> { createDto.LicensePlateNumber };
                vehicleDto.StationId = licensePlate.StationId;
                vehicleDto.StationName = vehicleWithDetails.LicensePlates.FirstOrDefault()?.Station?.StationName;
                vehicleDto.StationStreet = vehicleWithDetails.LicensePlates.FirstOrDefault()?.Station?.Street;

                _logger.LogInformation("Vehicle created successfully with ID: {VehicleId}", createdVehicle.VehicleId);

                return new VehicleResponseDto
                {
                    Success = true,
                    Message = "Vehicle created successfully",
                    Data = vehicleDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating vehicle");
                return new VehicleResponseDto
                {
                    Success = false,
                    Message = "An error occurred while creating the vehicle."
                };
            }
        }

        public async Task<VehicleResponseDto> UpdateAsync(int id, UpdateVehicleDto updateDto)
        {
            try
            {
                // Kiểm tra vehicle có tồn tại không
                var existingVehicle = await _vehicleRepository.GetByIdAsync(id);
                if (existingVehicle == null)
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy xe với ID này."
                    };
                }


                var brand = await _brandRepository.GetByIdAsync(updateDto.BrandId);
                if (brand == null)
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Brand not found"
                    };
                }

                if (updateDto.VehicleImageFile != null)
                {
                    try
                    {

                        if (!string.IsNullOrEmpty(existingVehicle.VehicleImage))
                        {
                            await _cloudService.DeleteImageAsync(existingVehicle.VehicleImage);
                        }

                        var newImageUrl = await _cloudService.UploadImageAsync(updateDto.VehicleImageFile);
                        existingVehicle.VehicleImage = newImageUrl;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading vehicle image for update");
                        return new VehicleResponseDto
                        {
                            Success = false,
                            Message = "Lỗi khi upload ảnh xe. Vui lòng thử lại."
                        };
                    }
                }

                existingVehicle.Model = updateDto.Model;
                existingVehicle.ModelYear = updateDto.ModelYear;
                existingVehicle.BrandId = updateDto.BrandId;
                existingVehicle.Description = updateDto.Description;
                existingVehicle.PricePerDay = updateDto.PricePerDay;
                existingVehicle.SeatNumber = updateDto.SeatNumber;
                existingVehicle.Battery = updateDto.Battery;
                existingVehicle.RangeKm = updateDto.RangeKm;

                var updatedVehicle = await _vehicleRepository.UpdateAsync(existingVehicle);

                // Xử lý chuyển trạm nếu có StationId trong request
                if (updateDto.StationId.HasValue)
                {
                    _logger.LogInformation("Starting vehicle station transfer for VehicleId: {VehicleId} to StationId: {StationId}", id, updateDto.StationId.Value);
                    
                    // Kiểm tra trạm mới có tồn tại không
                    var newStation = await _stationRepository.GetByIdAsync(updateDto.StationId.Value);
                    if (newStation == null)
                    {
                        _logger.LogWarning("Station not found for StationId: {StationId}", updateDto.StationId.Value);
                        return new VehicleResponseDto
                        {
                            Success = false,
                            Message = "Trạm mới không tồn tại"
                        };
                    }

                    var licensePlates = await _licensePlateRepository.GetLicensePlatesByVehicleIdAsync(id);
                    _logger.LogInformation("Found {Count} license plates for VehicleId: {VehicleId}", licensePlates.Count(), id);
                    
                    if (licensePlates.Any())
                    {
                        // Lưu thông tin trạm cũ và status TRƯỚC KHI cập nhật
                        var oldStationCounts = licensePlates
                            .GroupBy(lp => lp.StationId)
                            .ToDictionary(g => g.Key, g => g.Count());
                        
                        var availableCount = licensePlates.Count(lp => lp.Status == "Available");
                        
                        _logger.LogInformation("Before transfer - Old station counts: {StationCounts}, Available vehicles: {AvailableCount}", 
                            string.Join(", ", oldStationCounts.Select(kvp => $"Station {kvp.Key}: {kvp.Value} vehicles")), availableCount);

                        // Cập nhật tất cả license plates sang trạm mới
                        foreach (var licensePlate in licensePlates)
                        {
                            licensePlate.StationId = updateDto.StationId.Value;
                            _licensePlateRepository.Update(licensePlate);
                        }

                        // Cập nhật bộ đếm trạm cũ (giảm)
                        foreach (var kvp in oldStationCounts)
                        {
                            var oldStationId = kvp.Key;
                            var vehiclesFromThisStation = kvp.Value;
                            
                            if (oldStationId > 0)
                            {
                                var oldStation = await _stationRepository.GetByIdAsync(oldStationId);
                                if (oldStation != null)
                                {
                                    var oldAvailableCount = oldStation.AvailableVehicle;
                                    oldStation.AvailableVehicle = Math.Max(0, oldStation.AvailableVehicle - vehiclesFromThisStation);
                                    _stationRepository.Update(oldStation);
                                    _logger.LogInformation("Station {StationId} available vehicles: {OldCount} -> {NewCount} (decreased by {Decreased})", 
                                        oldStationId, oldAvailableCount, oldStation.AvailableVehicle, vehiclesFromThisStation);
                                }
                            }
                        }

                        // Cập nhật bộ đếm trạm mới (tăng)
                        var oldNewStationAvailable = newStation.AvailableVehicle;
                        newStation.AvailableVehicle = Math.Min(newStation.TotalVehicle, newStation.AvailableVehicle + availableCount);
                        _stationRepository.Update(newStation);
                        _logger.LogInformation("Station {StationId} available vehicles: {OldCount} -> {NewCount} (increased by {Increased})", 
                            updateDto.StationId.Value, oldNewStationAvailable, newStation.AvailableVehicle, availableCount);

                        // Lưu tất cả thay đổi vào database
                        await _unitOfWork.SaveChangesAsync();

                        _logger.LogInformation("Vehicle {VehicleId} transferred to station {StationId} successfully", id, updateDto.StationId.Value);
                    }
                }

                // Map sang DTO để trả về
                var vehicleDto = _mapper.Map<VehicleDto>(updatedVehicle);
                vehicleDto.BrandName = brand.BrandName;
                vehicleDto.Status = updatedVehicle.LicensePlates.FirstOrDefault()?.Status ?? "Available";
                vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(id);
                vehicleDto.AvailableLicensePlates = updatedVehicle.LicensePlates.Count(lp => lp.Status == "Available");
                vehicleDto.LicensePlateNumbers = updatedVehicle.LicensePlates.Select(lp => lp.PlateNumber).ToList();
                var firstPlateAfterUpdate = updatedVehicle.LicensePlates.FirstOrDefault();
                if (firstPlateAfterUpdate != null)
                {
                    vehicleDto.StationId = firstPlateAfterUpdate.StationId;
                    vehicleDto.StationName = firstPlateAfterUpdate.Station?.StationName;
                    vehicleDto.StationStreet = firstPlateAfterUpdate.Station?.Street;
                }

                _logger.LogInformation("Vehicle updated successfully with ID: {VehicleId}", id);

                return new VehicleResponseDto
                {
                    Success = true,
                    Message = "Vehicle updated successfully",
                    Data = vehicleDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating vehicle with ID: {Id}", id);
                return new VehicleResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật thông tin xe."
                };
            }
        }

        public async Task<VehicleResponseDto> DeleteAsync(int id)
        {
            try
            {
                // Kiểm tra vehicle có tồn tại không
                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy xe với ID này."
                    };
                }

                // Kiểm tra xe có đang được sử dụng không (có order đang active)
                // TODO: Thêm logic kiểm tra order active

                // Xóa tất cả LicensePlate của xe trước
                var licensePlates = await _licensePlateRepository.GetLicensePlatesByVehicleIdAsync(id);
                foreach (var licensePlate in licensePlates)
                {
                    await _licensePlateRepository.RemoveByIdAsync(licensePlate.LicensePlateId);
                }

                // Xóa ảnh khỏi cloud nếu có
                if (!string.IsNullOrEmpty(vehicle.VehicleImage))
                {
                    try
                    {
                        await _cloudService.DeleteImageAsync(vehicle.VehicleImage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete vehicle image from cloud for vehicle ID: {VehicleId}", id);
                        // Không dừng quá trình xóa nếu không xóa được ảnh
                    }
                }

                // Xóa xe
                var result = await _vehicleRepository.DeleteAsync(id);
                if (!result)
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Không thể xóa xe này."
                    };
                }

                // Cập nhật bộ đếm của trạm sau khi xóa
                // Giảm bộ đếm dựa trên Station của biển số (nếu có)
                var anyPlate = licensePlates.FirstOrDefault();
                if (anyPlate != null)
                {
                    var station = await _unitOfWork.Stations.GetByIdAsync(anyPlate.StationId);
                    if (station != null)
                    {
                        station.TotalVehicle = Math.Max(0, station.TotalVehicle - 1);
                        var wasAvailable = licensePlates.Any(lp => lp.Status == "Available") || (!licensePlates.Any());
                        if (wasAvailable)
                        {
                            station.AvailableVehicle = Math.Max(0, station.AvailableVehicle - 1);
                        }
                        _unitOfWork.Stations.Update(station);
                    }
                }

                // Commit tất cả thay đổi
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Vehicle deleted successfully with ID: {VehicleId}", id);

                return new VehicleResponseDto
                {
                    Success = true,
                    Message = "Vehicle deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting vehicle with ID: {Id}", id);
                return new VehicleResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi xóa xe."
                };
            }
        }


        public async Task<VehicleListResponseDto> GetAvailableVehiclesAsync()
        {
            try
            {
                var vehicles = await _vehicleRepository.GetAvailableVehiclesAsync();
                var vehicleDtos = new List<VehicleDto>();

                foreach (var vehicle in vehicles)
                {
                    var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
                    vehicleDto.Status = vehicle.LicensePlates.FirstOrDefault()?.Status ?? "Available";
                    vehicleDto.IsAvailable = true;
                    vehicleDto.AvailableLicensePlates = vehicle.LicensePlates.Count(lp => lp.Status == "Available");
                    vehicleDto.LicensePlateNumbers = vehicle.LicensePlates.Select(lp => lp.PlateNumber).ToList();
                    var firstPlate = vehicle.LicensePlates.FirstOrDefault();
                    if (firstPlate != null)
                    {
                        vehicleDto.StationId = firstPlate.StationId;
                        vehicleDto.StationName = firstPlate.Station?.StationName;
                        vehicleDto.StationStreet = firstPlate.Station?.Street;
                    }
                    vehicleDtos.Add(vehicleDto);
                }

                return new VehicleListResponseDto
                {
                    Success = true,
                    Message = "Available vehicles retrieved successfully.",
                    Data = vehicleDtos,
                    TotalCount = vehicleDtos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting available vehicles");
                return new VehicleListResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách xe có sẵn."
                };
            }
        }

        public async Task<VehicleListResponseDto> GetVehiclesByStationAsync(int stationId)
        {
            try
            {
                var vehicles = await _vehicleRepository.GetByStationIdAsync(stationId);
                var vehicleDtos = new List<VehicleDto>();

                foreach (var vehicle in vehicles)
                {
                    var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
                    vehicleDto.Status = vehicle.LicensePlates.FirstOrDefault()?.Status ?? "Available";
                    vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(vehicle.VehicleId);
                    vehicleDto.AvailableLicensePlates = vehicle.LicensePlates.Count(lp => lp.Status == "Available");
                    vehicleDto.LicensePlateNumbers = vehicle.LicensePlates.Select(lp => lp.PlateNumber).ToList();
                    var firstPlate = vehicle.LicensePlates.FirstOrDefault();
                    if (firstPlate != null)
                    {
                        vehicleDto.StationId = firstPlate.StationId;
                        vehicleDto.StationName = firstPlate.Station?.StationName;
                        vehicleDto.StationStreet = firstPlate.Station?.Street;
                    }
                    vehicleDtos.Add(vehicleDto);
                }

                return new VehicleListResponseDto
                {
                    Success = true,
                    Message = "Vehicles by station retrieved successfully.",
                    Data = vehicleDtos,
                    TotalCount = vehicleDtos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicles by station ID: {StationId}", stationId);
                return new VehicleListResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách xe theo trạm."
                };
            }
        }

        public async Task<bool> IsVehicleAvailableAsync(int vehicleId)
        {
            try
            {
                return await _vehicleRepository.IsVehicleAvailableAsync(vehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking vehicle availability with ID: {Id}", vehicleId);
                return false;
            }
        }

        public async Task<int> GetAvailableVehicleCountAsync()
        {
            try
            {
                return await _vehicleRepository.GetAvailableVehicleCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting available vehicle count");
                return 0;
            }
        }

        public async Task<VehicleResponseDto> ToggleAvailabilityAsync(int vehicleId)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
                if (vehicle == null)
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy xe với ID này."
                    };
                }


                return new VehicleResponseDto
                {
                    Success = true,
                    Message = "Cập nhật trạng thái xe thành công."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling vehicle availability with ID: {Id}", vehicleId);
                return new VehicleResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật trạng thái xe."
                };
            }
        }
    }
}
