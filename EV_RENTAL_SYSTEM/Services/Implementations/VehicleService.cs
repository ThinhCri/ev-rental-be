using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(
            IVehicleRepository vehicleRepository,
            IBrandRepository brandRepository,
            IMapper mapper,
            ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<VehicleResponseDto> GetByIdAsync(int id)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                {
                    return new VehicleResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy xe với ID này."
                    };
                }

                var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
                vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(id);
                vehicleDto.AvailableLicensePlates = vehicle.LicensePlates.Count(lp => lp.Status == "Available");

                return new VehicleResponseDto
                {
                    Success = true,
                    Message = "Lấy thông tin xe thành công.",
                    Data = vehicleDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicle with ID: {Id}", id);
                return new VehicleResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy thông tin xe."
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
                    Message = "Lấy danh sách xe thành công.",
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
                        Message = "Thương hiệu không tồn tại."
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
                    Message = "Tạo xe mới thành công.",
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
                        Message = "Thương hiệu không tồn tại."
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
                    Message = "Cập nhật thông tin xe thành công.",
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
                    Message = "Xóa xe thành công."
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

        public async Task<VehicleListResponseDto> SearchVehiclesAsync(VehicleSearchDto searchDto)
        {
            try
            {
                var (vehicles, totalCount) = await _vehicleRepository.GetVehiclesWithPaginationAsync(searchDto);
                var vehicleDtos = new List<VehicleDto>();

                foreach (var vehicle in vehicles)
                {
                    var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
                    vehicleDto.IsAvailable = await _vehicleRepository.IsVehicleAvailableAsync(vehicle.VehicleId);
                    vehicleDto.AvailableLicensePlates = vehicle.LicensePlates.Count(lp => lp.Status == "Available");
                    vehicleDtos.Add(vehicleDto);
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

                return new VehicleListResponseDto
                {
                    Success = true,
                    Message = "Tìm kiếm xe thành công.",
                    Data = vehicleDtos,
                    TotalCount = totalCount,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching vehicles");
                return new VehicleListResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi tìm kiếm xe."
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

        public async Task<VehicleListResponseDto> GetVehiclesByBrandAsync(int brandId)
        {
            try
            {
                var vehicles = await _vehicleRepository.GetByBrandIdAsync(brandId);
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
                    Message = "Lấy danh sách xe theo thương hiệu thành công.",
                    Data = vehicleDtos,
                    TotalCount = vehicleDtos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicles by brand ID: {BrandId}", brandId);
                return new VehicleListResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách xe theo thương hiệu."
                };
            }
        }

        public async Task<VehicleListResponseDto> GetVehiclesByTypeAsync(string vehicleType)
        {
            try
            {
                var vehicles = await _vehicleRepository.GetByVehicleTypeAsync(vehicleType);
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
                    Message = "Lấy danh sách xe theo loại thành công.",
                    Data = vehicleDtos,
                    TotalCount = vehicleDtos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicles by type: {VehicleType}", vehicleType);
                return new VehicleListResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách xe theo loại."
                };
            }
        }

        public async Task<VehicleListResponseDto> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            try
            {
                var vehicles = await _vehicleRepository.GetVehiclesByPriceRangeAsync(minPrice, maxPrice);
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
                    Message = "Lấy danh sách xe theo khoảng giá thành công.",
                    Data = vehicleDtos,
                    TotalCount = vehicleDtos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicles by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
                return new VehicleListResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách xe theo khoảng giá."
                };
            }
        }

        public async Task<VehicleListResponseDto> GetVehiclesBySeatRangeAsync(int minSeats, int maxSeats)
        {
            try
            {
                var vehicles = await _vehicleRepository.GetVehiclesBySeatRangeAsync(minSeats, maxSeats);
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
                    Message = "Lấy danh sách xe theo số ghế thành công.",
                    Data = vehicleDtos,
                    TotalCount = vehicleDtos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicles by seat range: {MinSeats} - {MaxSeats}", minSeats, maxSeats);
                return new VehicleListResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách xe theo số ghế."
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
