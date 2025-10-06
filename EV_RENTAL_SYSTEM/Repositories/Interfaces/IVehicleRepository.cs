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
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
        Task<IEnumerable<Vehicle>> GetByStationIdAsync(int stationId);
        
        // Business logic queries
        Task<bool> IsVehicleAvailableAsync(int vehicleId);
        Task<int> GetAvailableVehicleCountAsync();
    }
}



