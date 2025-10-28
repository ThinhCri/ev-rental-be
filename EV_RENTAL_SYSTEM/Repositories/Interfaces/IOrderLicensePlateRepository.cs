using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IOrderLicensePlateRepository : IGenericRepository<Order_LicensePlate>
    {
        Task<IEnumerable<Order_LicensePlate>> GetOrderLicensePlatesByOrderIdAsync(int orderId);
        Task<IEnumerable<Order_LicensePlate>> GetOrderLicensePlatesByLicensePlateIdAsync(string licensePlateId);
        Task<bool> IsLicensePlateInOrderAsync(string licensePlateId, DateTime startTime, DateTime endTime);
    }
}
