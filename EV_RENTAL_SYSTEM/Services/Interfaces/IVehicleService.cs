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
        Task<VehicleListResponseDto> SearchVehiclesAsync(VehicleSearchDto searchDto);
        Task<VehicleListResponseDto> GetAvailableVehiclesAsync();
        Task<VehicleListResponseDto> GetVehiclesByBrandAsync(int brandId);
        Task<VehicleListResponseDto> GetVehiclesByTypeAsync(string vehicleType);
        Task<VehicleListResponseDto> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<VehicleListResponseDto> GetVehiclesBySeatRangeAsync(int minSeats, int maxSeats);
        
        // Business logic
        Task<bool> IsVehicleAvailableAsync(int vehicleId);
        Task<int> GetAvailableVehicleCountAsync();
        Task<VehicleResponseDto> ToggleAvailabilityAsync(int vehicleId);
    }
}
