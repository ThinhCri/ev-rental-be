using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface ILicensePlateRepository : IGenericRepository<LicensePlate>
    {
        Task<IEnumerable<LicensePlate>> GetLicensePlatesByVehicleIdAsync(int vehicleId);
        Task<IEnumerable<LicensePlate>> GetAvailableLicensePlatesAsync();
        Task<LicensePlate?> GetLicensePlateByNumberAsync(string plateNumber);
        Task<int> GetVehiclesByStationIdAsync(int stationId);
        Task<int> GetAvailableVehiclesByStationIdAsync(int stationId);
    }
}
