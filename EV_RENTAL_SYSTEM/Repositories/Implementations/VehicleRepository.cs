using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                    .ThenInclude(lp => lp.Station)
                .FirstOrDefaultAsync(v => v.VehicleId == id);
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                    .ThenInclude(lp => lp.Station)
                .ToListAsync();
        }

        public async Task<Vehicle> AddAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return false;

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Vehicles.AnyAsync(v => v.VehicleId == id);
        }

        public async Task<IEnumerable<Vehicle>> GetByStationIdAsync(int stationId)
        {
            // Vehicles that have at least one license plate at the given station
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                    .ThenInclude(lp => lp.Station)
                .Where(v => v.LicensePlates.Any(lp => lp.StationId == stationId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            // Lấy các xe có ít nhất 1 biển số có status "Available" HOẶC chưa có biển số nào (xe mới)
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                    .ThenInclude(lp => lp.Station)
                .Where(v => !v.LicensePlates.Any() || v.LicensePlates.Any(lp => lp.Status == "Available"))
                .ToListAsync();
        }


        public async Task<bool> IsVehicleAvailableAsync(int vehicleId)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.LicensePlates)
                .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);

            if (vehicle == null) return false;

            // Nếu xe chưa có biển số nào, coi như Available (xe mới)
            if (!vehicle.LicensePlates.Any())
                return true;

            // Nếu có biển số, kiểm tra có biển số nào Available không
            return vehicle.LicensePlates.Any(lp => lp.Status == "Available");
        }

        public async Task<int> GetAvailableVehicleCountAsync()
        {
            return await _context.Vehicles
                .Where(v => !v.LicensePlates.Any() || v.LicensePlates.Any(lp => lp.Status == "Available"))
                .CountAsync();
        }
    }
}
