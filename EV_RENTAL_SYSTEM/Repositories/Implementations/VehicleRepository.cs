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

        public async Task<IEnumerable<Vehicle>> GetByBrandIdAsync(int brandId)
        {
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                .Where(v => v.BrandId == brandId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetByVehicleTypeAsync(string vehicleType)
        {
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                .Where(v => v.VehicleType == vehicleType)
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

        public async Task<IEnumerable<Vehicle>> SearchVehiclesAsync(VehicleSearchDto searchDto)
        {
            var query = _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.Model))
            {
                query = query.Where(v => v.Model.Contains(searchDto.Model));
            }

            if (searchDto.BrandId.HasValue)
            {
                query = query.Where(v => v.BrandId == searchDto.BrandId.Value);
            }

            if (!string.IsNullOrEmpty(searchDto.VehicleType))
            {
                query = query.Where(v => v.VehicleType == searchDto.VehicleType);
            }

            if (searchDto.MinPricePerDay.HasValue)
            {
                query = query.Where(v => v.PricePerDay >= searchDto.MinPricePerDay.Value);
            }

            if (searchDto.MaxPricePerDay.HasValue)
            {
                query = query.Where(v => v.PricePerDay <= searchDto.MaxPricePerDay.Value);
            }

            if (searchDto.MinSeatNumber.HasValue)
            {
                query = query.Where(v => v.SeatNumber >= searchDto.MinSeatNumber.Value);
            }

            if (searchDto.MaxSeatNumber.HasValue)
            {
                query = query.Where(v => v.SeatNumber <= searchDto.MaxSeatNumber.Value);
            }

            if (searchDto.IsAvailable.HasValue && searchDto.IsAvailable.Value)
            {
                query = query.Where(v => !v.LicensePlates.Any() || v.LicensePlates.Any(lp => lp.Status == "Available"));
            }

            // Apply sorting
            switch (searchDto.SortBy?.ToLower())
            {
                case "dailyrate":
                    query = searchDto.SortOrder?.ToLower() == "desc" 
                        ? query.OrderByDescending(v => v.PricePerDay)
                        : query.OrderBy(v => v.PricePerDay);
                    break;
                case "modelyear":
                    query = searchDto.SortOrder?.ToLower() == "desc" 
                        ? query.OrderByDescending(v => v.ModelYear)
                        : query.OrderBy(v => v.ModelYear);
                    break;
                default: // Model
                    query = searchDto.SortOrder?.ToLower() == "desc" 
                        ? query.OrderByDescending(v => v.Model)
                        : query.OrderBy(v => v.Model);
                    break;
            }

            return await query.ToListAsync();
        }

        public async Task<(IEnumerable<Vehicle> vehicles, int totalCount)> GetVehiclesWithPaginationAsync(VehicleSearchDto searchDto)
        {
            var query = _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                .AsQueryable();

            // Apply filters (same as SearchVehiclesAsync)
            if (!string.IsNullOrEmpty(searchDto.Model))
            {
                query = query.Where(v => v.Model.Contains(searchDto.Model));
            }

            if (searchDto.BrandId.HasValue)
            {
                query = query.Where(v => v.BrandId == searchDto.BrandId.Value);
            }

            if (!string.IsNullOrEmpty(searchDto.VehicleType))
            {
                query = query.Where(v => v.VehicleType == searchDto.VehicleType);
            }

            if (searchDto.MinPricePerDay.HasValue)
            {
                query = query.Where(v => v.PricePerDay >= searchDto.MinPricePerDay.Value);
            }

            if (searchDto.MaxPricePerDay.HasValue)
            {
                query = query.Where(v => v.PricePerDay <= searchDto.MaxPricePerDay.Value);
            }

            if (searchDto.MinSeatNumber.HasValue)
            {
                query = query.Where(v => v.SeatNumber >= searchDto.MinSeatNumber.Value);
            }

            if (searchDto.MaxSeatNumber.HasValue)
            {
                query = query.Where(v => v.SeatNumber <= searchDto.MaxSeatNumber.Value);
            }

            if (searchDto.IsAvailable.HasValue && searchDto.IsAvailable.Value)
            {
                query = query.Where(v => !v.LicensePlates.Any() || v.LicensePlates.Any(lp => lp.Status == "Available"));
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply sorting
            switch (searchDto.SortBy?.ToLower())
            {
                case "dailyrate":
                    query = searchDto.SortOrder?.ToLower() == "desc" 
                        ? query.OrderByDescending(v => v.PricePerDay)
                        : query.OrderBy(v => v.PricePerDay);
                    break;
                case "modelyear":
                    query = searchDto.SortOrder?.ToLower() == "desc" 
                        ? query.OrderByDescending(v => v.ModelYear)
                        : query.OrderBy(v => v.ModelYear);
                    break;
                default: // Model
                    query = searchDto.SortOrder?.ToLower() == "desc" 
                        ? query.OrderByDescending(v => v.Model)
                        : query.OrderBy(v => v.Model);
                    break;
            }

            // Apply pagination
            var vehicles = await query
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            return (vehicles, totalCount);
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                .Where(v => v.PricePerDay >= minPrice && v.PricePerDay <= maxPrice)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesBySeatRangeAsync(int minSeats, int maxSeats)
        {
            return await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.LicensePlates)
                .Where(v => v.SeatNumber >= minSeats && v.SeatNumber <= maxSeats)
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
