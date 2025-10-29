using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class LicensePlateRepository : GenericRepository<LicensePlate>, ILicensePlateRepository
    {
        public LicensePlateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LicensePlate>> GetLicensePlatesByVehicleIdAsync(int vehicleId)
        {
            return await _context.LicensePlates
                .Where(lp => lp.VehicleId == vehicleId)
                .Include(lp => lp.Vehicle)
                .ThenInclude(v => v.Brand)
                .Include(lp => lp.Station)
                .ToListAsync();
        }

        public async Task<IEnumerable<LicensePlate>> GetAvailableLicensePlatesAsync()
        {
            return await _context.LicensePlates
                .Where(lp => lp.Status == "Available")
                .Include(lp => lp.Vehicle)
                .ThenInclude(v => v.Brand)
                .Include(lp => lp.Station)
                .ToListAsync();
        }

        public async Task<LicensePlate?> GetLicensePlateByNumberAsync(string plateNumber)
        {
            return await _context.LicensePlates
                .Include(lp => lp.Vehicle)
                .ThenInclude(v => v.Brand)
                .Include(lp => lp.Station)
                .FirstOrDefaultAsync(lp => lp.PlateNumber == plateNumber);
        }

        public async Task<int> GetVehiclesByStationIdAsync(int stationId)
        {
            return await _context.LicensePlates
                .Where(lp => lp.StationId == stationId)
                .Select(lp => lp.VehicleId)
                .Distinct()
                .CountAsync();
        }

        public async Task<int> GetAvailableVehiclesByStationIdAsync(int stationId)
        {
            return await _context.LicensePlates
                .Where(lp => lp.StationId == stationId && lp.Status == "Available")
                .Select(lp => lp.VehicleId)
                .Distinct()
                .CountAsync();
        }
    }
}
