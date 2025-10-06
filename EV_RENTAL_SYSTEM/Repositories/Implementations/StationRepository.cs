using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class StationRepository : GenericRepository<Station>, IStationRepository
    {
        public StationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Station>> GetStationsByProvinceAsync(string province)
        {
            return await _context.Stations
                .Where(s => s.Province == province)
                .ToListAsync();
        }

        public async Task<Station?> GetStationWithVehiclesAsync(int stationId)
        {
            return await _context.Stations
                .Include(s => s.Vehicles)
                    .ThenInclude(v => v.Brand)
                .Include(s => s.Vehicles)
                    .ThenInclude(v => v.LicensePlates)
                .FirstOrDefaultAsync(s => s.StationId == stationId);
        }
    }
}




