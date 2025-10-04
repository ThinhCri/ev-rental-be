using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
        Task<VehicleDto?> GetVehicleByIdAsync(int id);
        Task<VehicleDto> CreateVehicleAsync(VehicleDto vehicle);
        Task<VehicleDto?> UpdateVehicleAsync(int id, VehicleDto vehicle);
        Task<bool> DeleteVehicleAsync(int id);
    }
}
