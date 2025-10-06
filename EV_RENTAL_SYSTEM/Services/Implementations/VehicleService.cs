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
        private readonly IMapper _mapper;

        public VehicleService(
            IVehicleRepository vehicleRepository,
            IBrandRepository brandRepository,
            IMapper mapper,
            ILogger<VehicleService> logger) : base(logger)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _mapper = mapper;
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
                vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(id);
                vehicleDto.AvailableLicensePlates = vehicle.LicensePlates.Count(lp => lp.Status == "Available");

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
                    vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(vehicle.VehicleId);
                    vehicleDto.AvailableLicensePlates = vehicle.LicensePlates.Count(lp => lp.Status == "Available");
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

                // Tạo vehicle entity
                var vehicle = _mapper.Map<Vehicle>(createDto);
                vehicle.Brand = brand;

                // Lưu vào database
                var createdVehicle = await _vehicleRepository.AddAsync(vehicle);

                // Map sang DTO để trả về
                var vehicleDto = _mapper.Map<VehicleDto>(createdVehicle);
                vehicleDto.BrandName = brand.BrandName;
                vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(createdVehicle.VehicleId);
                vehicleDto.AvailableLicensePlates = createdVehicle.LicensePlates.Count(lp => lp.Status == "Available");

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
                    Message = "Đã xảy ra lỗi khi tạo xe mới."
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

                // Kiểm tra brand có tồn tại không
                var brand = await _brandRepository.GetByIdAsync(updateDto.BrandId);
                if (brand == null)
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Brand not found"
                    };
                }

                // Cập nhật thông tin
                existingVehicle.Model = updateDto.Model;
                existingVehicle.ModelYear = updateDto.ModelYear;
                existingVehicle.BrandId = updateDto.BrandId;
                existingVehicle.VehicleType = updateDto.VehicleType;
                existingVehicle.Description = updateDto.Description;
                existingVehicle.PricePerDay = updateDto.PricePerDay;
                existingVehicle.SeatNumber = updateDto.SeatNumber;

                // Lưu vào database
                var updatedVehicle = await _vehicleRepository.UpdateAsync(existingVehicle);

                // Map sang DTO để trả về
                var vehicleDto = _mapper.Map<VehicleDto>(updatedVehicle);
                vehicleDto.BrandName = brand.BrandName;
                vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(id);
                vehicleDto.AvailableLicensePlates = updatedVehicle.LicensePlates.Count(lp => lp.Status == "Available");

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
                    vehicleDto.IsAvailable = true;
                    vehicleDto.AvailableLicensePlates = vehicle.LicensePlates.Count(lp => lp.Status == "Available");
                    vehicleDtos.Add(vehicleDto);
                }

                return new VehicleListResponseDto
                {
                    Success = true,
                    Message = "Lấy danh sách xe có sẵn thành công.",
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
                    vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(vehicle.VehicleId);
                    vehicleDto.AvailableLicensePlates = vehicle.LicensePlates.Count(lp => lp.Status == "Available");
                    vehicleDtos.Add(vehicleDto);
                }

                return new VehicleListResponseDto
                {
                    Success = true,
                    Message = "Lấy danh sách xe theo trạm thành công.",
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

                // TODO: Implement logic to toggle availability
                // This might involve updating license plate statuses

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
