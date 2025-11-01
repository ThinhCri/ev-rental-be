using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IOrderLicensePlateRepository : IGenericRepository<Order_LicensePlate>
    {
        Task<IEnumerable<Order_LicensePlate>> GetOrderLicensePlatesByOrderIdAsync(int orderId);
        Task<IEnumerable<Order_LicensePlate>> GetOrderLicensePlatesByLicensePlateIdAsync(int licensePlateId);
        Task<IEnumerable<Order_LicensePlate>> GetByOrderIdAsync(int orderId);
        Task<bool> IsLicensePlateInOrderAsync(int licensePlateId, DateTime startTime, DateTime endTime);
    }
}
