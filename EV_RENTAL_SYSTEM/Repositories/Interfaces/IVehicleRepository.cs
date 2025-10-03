using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        // Basic CRUD operations
        Task<Vehicle?> GetByIdAsync(int id);
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<Vehicle> AddAsync(Vehicle vehicle);
        Task<Vehicle> UpdateAsync(Vehicle vehicle);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Advanced queries
        Task<IEnumerable<Vehicle>> GetByBrandIdAsync(int brandId);
        Task<IEnumerable<Vehicle>> GetByVehicleTypeAsync(string vehicleType);
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
        Task<IEnumerable<Vehicle>> SearchVehiclesAsync(VehicleSearchDto searchDto);
        Task<(IEnumerable<Vehicle> vehicles, int totalCount)> GetVehiclesWithPaginationAsync(VehicleSearchDto searchDto);
        
        // Business logic queries
        Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Vehicle>> GetVehiclesBySeatRangeAsync(int minSeats, int maxSeats);
        Task<bool> IsVehicleAvailableAsync(int vehicleId);
        Task<int> GetAvailableVehicleCountAsync();
    }
}
