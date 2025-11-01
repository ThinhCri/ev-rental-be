using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class MaintenanceRepository : GenericRepository<Maintenance>, IMaintenanceRepository
    {
        public MaintenanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Maintenance>> GetMaintenancesByLicensePlateIdAsync(int licensePlateId)
        {
            return await Task.FromResult(_context.Set<Maintenance>()
                .Where(m => m.LicensePlateId == licensePlateId)
                .ToList());
        }

        public async Task<IEnumerable<Maintenance>> GetMaintenancesByStatusAsync(string status)
        {
            return await Task.FromResult(_context.Set<Maintenance>()
                .Where(m => m.Status == status)
                .ToList());
        }

        public async Task<IEnumerable<Maintenance>> GetMaintenancesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.FromResult(_context.Set<Maintenance>()
                .Where(m => m.MaintenanceDate >= startDate && m.MaintenanceDate <= endDate)
                .ToList());
        }
    }
}

