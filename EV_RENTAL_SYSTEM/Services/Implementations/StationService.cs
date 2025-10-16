using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<StationService> _logger;

        public StationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<StationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<StationListResponseDto> GetAllAsync()
        {
            try
            {
                await UpdateAllStationCountersAsync();
                var stations = await _unitOfWork.Stations.GetAllAsync();
                var stationDtos = stations.Select(MapToStationDto).ToList();

                return new StationListResponseDto
                {
                    Success = true,
                    Message = "Lấy danh sách trạm thành công",
                    Data = stationDtos,
                    TotalCount = stationDtos.Count,
                    PageNumber = 1,
                    PageSize = stationDtos.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all stations");
                return new StationListResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi lấy danh sách trạm",
                    Data = new List<StationDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<StationResponseDto> GetByIdAsync(int id)
        {
            try
            {
                var station = await _unitOfWork.Stations.GetByIdAsync(id);
                if (station == null)
                {
                    return new StationResponseDto
                    {
                        Success = false,
                        Message = "Station not found"
                    };
                }

                var stationDto = MapToStationDto(station);

                return new StationResponseDto
                {
                    Success = true,
                    Message = "Lấy thông tin trạm thành công",
                    Data = stationDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting station by id: {StationId}", id);
                return new StationResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi lấy thông tin trạm"
                };
            }
        }

        public async Task<StationResponseDto> CreateAsync(CreateStationDto createDto)
        {
            try
            {
                var station = _mapper.Map<Station>(createDto);
                await _unitOfWork.Stations.AddAsync(station);
                await _unitOfWork.SaveChangesAsync();

                var stationDto = MapToStationDto(station);

                _logger.LogInformation("Created station {StationId}: {StationName}", station.StationId, station.StationName);

                return new StationResponseDto
                {
                    Success = true,
                    Message = "Tạo trạm thành công",
                    Data = stationDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating station");
                return new StationResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi tạo trạm"
                };
            }
        }

        public async Task<StationResponseDto> UpdateAsync(int id, UpdateStationDto updateDto)
        {
            try
            {
                var station = await _unitOfWork.Stations.GetByIdAsync(id);
                if (station == null)
                {
                    return new StationResponseDto
                    {
                        Success = false,
                        Message = "Station not found"
                    };
                }

                _mapper.Map(updateDto, station);
                _unitOfWork.Stations.Update(station);
                await _unitOfWork.SaveChangesAsync();

                var stationDto = MapToStationDto(station);

                _logger.LogInformation("Updated station {StationId}: {StationName}", station.StationId, station.StationName);

                return new StationResponseDto
                {
                    Success = true,
                    Message = "Cập nhật trạm thành công",
                    Data = stationDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating station {StationId}", id);
                return new StationResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi cập nhật trạm"
                };
            }
        }

        public async Task<StationResponseDto> DeleteAsync(int id)
        {
            try
            {
                var station = await _unitOfWork.Stations.GetByIdAsync(id);
                if (station == null)
                {
                    return new StationResponseDto
                    {
                        Success = false,
                        Message = "Station not found"
                    };
                }

                // Kiểm tra xem trạm có xe không (qua LicensePlates)
                var licensePlates = await _unitOfWork.LicensePlates.GetAllAsync();
                var stationVehicles = licensePlates
                    .Where(lp => lp.StationId == id)
                    .Select(lp => lp.VehicleId)
                    .Distinct()
                    .ToList();
                
                if (stationVehicles.Any())
                {
                    return new StationResponseDto
                    {
                        Success = false,
                        Message = "Không thể xóa trạm vì còn có xe trong trạm"
                    };
                }

                _unitOfWork.Stations.Remove(station);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Deleted station {StationId}: {StationName}", station.StationId, station.StationName);

                return new StationResponseDto
                {
                    Success = true,
                    Message = "Xóa trạm thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting station {StationId}", id);
                return new StationResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi xóa trạm"
                };
            }
        }

        public async Task<StationListResponseDto> GetStationsByProvinceAsync(string province)
        {
            try
            {
                var stations = await _unitOfWork.Stations.GetStationsByProvinceAsync(province);
                var stationDtos = stations.Select(MapToStationDto).ToList();

                return new StationListResponseDto
                {
                    Success = true,
                    Message = $"Lấy danh sách trạm ở {province} thành công",
                    Data = stationDtos,
                    TotalCount = stationDtos.Count,
                    PageNumber = 1,
                    PageSize = stationDtos.Count,
                    TotalPages = 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stations by province: {Province}", province);
                return new StationListResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi lấy danh sách trạm theo tỉnh",
                    Data = new List<StationDto>(),
                    TotalCount = 0,
                    PageNumber = 1,
                    PageSize = 10,
                    TotalPages = 0
                };
            }
        }

        public async Task<StationResponseDto> GetStationWithVehiclesAsync(int stationId)
        {
            try
            {
                var station = await _unitOfWork.Stations.GetStationWithVehiclesAsync(stationId);
                if (station == null)
                {
                    return new StationResponseDto
                    {
                        Success = false,
                        Message = "Station not found"
                    };
                }

                var stationDto = MapToStationDto(station);
                // Tính từ LicensePlates
                var licensePlates = station.LicensePlates ?? new List<LicensePlate>();
                stationDto.VehicleCount = licensePlates.Select(lp => lp.VehicleId).Distinct().Count();
                stationDto.AvailableVehicleCount = licensePlates
                    .Where(lp => lp.Status == "Available")
                    .Select(lp => lp.VehicleId)
                    .Distinct()
                    .Count();

                return new StationResponseDto
                {
                    Success = true,
                    Message = "Station and vehicle information retrieved successfully",
                    Data = stationDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting station with vehicles: {StationId}", stationId);
                return new StationResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi lấy thông tin trạm và xe"
                };
            }
        }

        public async Task<StationListResponseDto> SearchStationsAsync(StationSearchDto searchDto)
        {
            try
            {
                var allStations = await _unitOfWork.Stations.GetAllAsync();
                var query = allStations.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchDto.StationName))
                {
                    query = query.Where(s => s.StationName != null && 
                        s.StationName.Contains(searchDto.StationName, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(searchDto.Province))
                {
                    query = query.Where(s => s.Province != null && 
                        s.Province.Contains(searchDto.Province, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(searchDto.District))
                {
                    query = query.Where(s => s.District != null && 
                        s.District.Contains(searchDto.District, StringComparison.OrdinalIgnoreCase));
                }

                // Apply sorting
                query = searchDto.SortBy?.ToLower() switch
                {
                    "province" => searchDto.SortOrder?.ToLower() == "desc" 
                        ? query.OrderByDescending(s => s.Province)
                        : query.OrderBy(s => s.Province),
                    "district" => searchDto.SortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(s => s.District)
                        : query.OrderBy(s => s.District),
                    _ => searchDto.SortOrder?.ToLower() == "desc"
                        ? query.OrderByDescending(s => s.StationName)
                        : query.OrderBy(s => s.StationName)
                };

                var totalCount = query.Count();
                var stations = query
                    .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                    .Take(searchDto.PageSize)
                    .ToList();

                var stationDtos = stations.Select(MapToStationDto).ToList();
                var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

                return new StationListResponseDto
                {
                    Success = true,
                    Message = "Tìm kiếm trạm thành công",
                    Data = stationDtos,
                    TotalCount = totalCount,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stations");
                return new StationListResponseDto
                {
                    Success = false,
                    Message = "Lỗi khi tìm kiếm trạm",
                    Data = new List<StationDto>(),
                    TotalCount = 0,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = 0
                };
            }
        }

        private StationDto MapToStationDto(Station station)
        {
            var stationDto = _mapper.Map<StationDto>(station);
            
            // Tạo địa chỉ đầy đủ
            var addressParts = new List<string>();
            if (!string.IsNullOrEmpty(station.Street)) addressParts.Add(station.Street);
            if (!string.IsNullOrEmpty(station.District)) addressParts.Add(station.District);
            if (!string.IsNullOrEmpty(station.Province)) addressParts.Add(station.Province);
            if (!string.IsNullOrEmpty(station.Country)) addressParts.Add(station.Country);
            
            stationDto.FullAddress = string.Join(", ", addressParts);
            
            // Gán số liệu đếm từ cột tổng hợp trong bảng Station
            stationDto.VehicleCount = station.TotalVehicle;
            stationDto.AvailableVehicleCount = station.AvailableVehicle;
            
            return stationDto;
        }

     
        public async Task UpdateAllStationCountersAsync()
        {
            try
            {
                _logger.LogInformation("Starting to update counters for all stations");
                
                var stations = await _unitOfWork.Stations.GetAllAsync();
                var updatedCount = 0;
                
                foreach (var station in stations)
                {
                    
                    var totalVehicles = await _unitOfWork.LicensePlates.GetVehiclesByStationIdAsync(station.StationId);
                    var availableVehicles = await _unitOfWork.LicensePlates.GetAvailableVehiclesByStationIdAsync(station.StationId);

                    if (station.TotalVehicle != totalVehicles || station.AvailableVehicle != availableVehicles)
                    {
                        var oldTotal = station.TotalVehicle;
                        var oldAvailable = station.AvailableVehicle;
                        
                        station.TotalVehicle = totalVehicles;
                        station.AvailableVehicle = availableVehicles;
                        
                        _unitOfWork.Stations.Update(station);
                        updatedCount++;
                        
                        _logger.LogInformation("Station {StationId} ({StationName}): Total {OldTotal}->{NewTotal}, Available {OldAvailable}->{NewAvailable}", 
                            station.StationId, station.StationName, oldTotal, totalVehicles, oldAvailable, availableVehicles);
                    }
                }
                
                if (updatedCount > 0)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Successfully updated counters for {Count} stations", updatedCount);
                }
                else
                {
                    _logger.LogInformation("No stations needed counter updates");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating all station counters");
                throw;
            }
        }
    }
}
