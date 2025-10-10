using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class OrderLicensePlateRepository : GenericRepository<Order_LicensePlate>, IOrderLicensePlateRepository
    {
        public OrderLicensePlateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order_LicensePlate>> GetOrderLicensePlatesByOrderIdAsync(int orderId)
        {
            return await _context.OrderLicensePlates
                .Where(olp => olp.OrderId == orderId)
                .Include(olp => olp.Order)
                .Include(olp => olp.LicensePlate)
                .ThenInclude(lp => lp.Vehicle)
                .ThenInclude(v => v.Brand)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order_LicensePlate>> GetOrderLicensePlatesByLicensePlateIdAsync(int licensePlateId)
        {
            return await _context.OrderLicensePlates
                .Where(olp => olp.LicensePlateId == licensePlateId)
                .Include(olp => olp.Order)
                .Include(olp => olp.LicensePlate)
                .ThenInclude(lp => lp.Vehicle)
                .ThenInclude(v => v.Brand)
                .ToListAsync();
        }

        public async Task<bool> IsLicensePlateInOrderAsync(int licensePlateId, DateTime startTime, DateTime endTime)
        {
            var conflictingOrders = await _context.OrderLicensePlates
                .Where(olp => olp.LicensePlateId == licensePlateId)
                .Include(olp => olp.Order)
                .Where(olp => olp.Order.Status != "Cancelled" && 
                             olp.Order.Status != "Completed" &&
                             olp.Order.StartTime.HasValue && 
                             olp.Order.EndTime.HasValue &&
                             !(endTime <= olp.Order.StartTime.Value || startTime >= olp.Order.EndTime.Value))
                .AnyAsync();

            return conflictingOrders;
        }
    }
}
