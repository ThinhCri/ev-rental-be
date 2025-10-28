<<<<<<< HEAD
﻿using EV_RENTAL_SYSTEM.Models.DTOs;
=======
using EV_RENTAL_SYSTEM.Models.DTOs;
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IVehicleService
    {
<<<<<<< HEAD
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
        Task<VehicleDto?> GetVehicleByIdAsync(int id);
        Task<VehicleDto> CreateVehicleAsync(VehicleDto vehicle);
        Task<VehicleDto?> UpdateVehicleAsync(int id, VehicleDto vehicle);
        Task<bool> DeleteVehicleAsync(int id);
    }
}
=======
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



>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
