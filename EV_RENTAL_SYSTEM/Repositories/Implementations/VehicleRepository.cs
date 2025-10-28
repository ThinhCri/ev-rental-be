<<<<<<< HEAD
﻿using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


=======
using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

<<<<<<< HEAD
        public async Task<IEnumerable<Vehicle>> GetAllAsync()
            => await _context.Vehicles.ToListAsync();

        public async Task<Vehicle?> GetByIdAsync(int id)
            => await _context.Vehicles.FindAsync(id);

        public async Task AddAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
        }
    }

=======
        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                .FirstOrDefaultAsync(v => v.VehicleId == id);
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
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
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Station)
                .Include(v => v.LicensePlates)
                .Where(v => v.StationId == stationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            // Lấy các xe có ít nhất 1 biển số có status "Available" HOẶC chưa có biển số nào (xe mới)
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
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
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
}
