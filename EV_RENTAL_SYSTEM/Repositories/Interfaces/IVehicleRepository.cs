<<<<<<< HEAD
﻿using EV_RENTAL_SYSTEM.Models;
=======
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
<<<<<<< HEAD
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<Vehicle?> GetByIdAsync(int id);
        Task AddAsync(Vehicle vehicle);
        Task UpdateAsync(Vehicle vehicle);
        Task DeleteAsync(int id);
    }
}
=======
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



>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
