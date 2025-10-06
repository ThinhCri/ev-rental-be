using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IVehicleService
    {
        // Basic CRUD operations
        Task<VehicleResponseDto> GetByIdAsync(int id);
        Task<VehicleListResponseDto> GetAllAsync();
        Task<VehicleResponseDto> CreateAsync(CreateVehicleDto createDto);
        Task<VehicleResponseDto> UpdateAsync(int id, UpdateVehicleDto updateDto);
        Task<VehicleResponseDto> DeleteAsync(int id);

        // Advanced operations
        Task<VehicleListResponseDto> GetAvailableVehiclesAsync();
        Task<VehicleListResponseDto> GetVehiclesByStationAsync(int stationId);
        
        // Business logic
        Task<bool> IsVehicleAvailableAsync(int vehicleId);
        Task<int> GetAvailableVehicleCountAsync();
        Task<VehicleResponseDto> ToggleAvailabilityAsync(int vehicleId);
    }
}



