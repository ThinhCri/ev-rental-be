using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IStationService
    {
        Task<StationListResponseDto> GetAllAsync();
        Task<StationResponseDto> GetByIdAsync(int id);
        Task<StationResponseDto> CreateAsync(CreateStationDto createDto);
        Task<StationResponseDto> UpdateAsync(int id, UpdateStationDto updateDto);
        Task<StationResponseDto> DeleteAsync(int id);
        Task<StationListResponseDto> GetStationsByProvinceAsync(string province);
        Task<StationResponseDto> GetStationWithVehiclesAsync(int stationId);
        Task<StationListResponseDto> SearchStationsAsync(StationSearchDto searchDto);
    }
}

