using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IMaintenanceRepository : IGenericRepository<Maintenance>
    {
        Task<IEnumerable<Maintenance>> GetMaintenancesByLicensePlateIdAsync(int licensePlateId);
        Task<IEnumerable<Maintenance>> GetMaintenancesByStatusAsync(string status);
        Task<IEnumerable<Maintenance>> GetMaintenancesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}

