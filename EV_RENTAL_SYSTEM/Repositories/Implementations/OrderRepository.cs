using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Override GetAllAsync to include related entities
        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                    .ThenInclude(u => u.Licenses)
                .Include(o => o.OrderLicensePlates)
                    .ThenInclude(olp => olp.LicensePlate)
                        .ThenInclude(lp => lp.Vehicle)
                            .ThenInclude(v => v.Brand)
                .ToListAsync();
        }

        // Override GetByIdAsync to include related entities
        public override async Task<Order?> GetByIdAsync(object id)
        {
            return await _context.Orders
                .Include(o => o.User)
                    .ThenInclude(u => u.Licenses)
                .Include(o => o.OrderLicensePlates)
                    .ThenInclude(olp => olp.LicensePlate)
                        .ThenInclude(lp => lp.Vehicle)
                            .ThenInclude(v => v.Brand)
                .FirstOrDefaultAsync(o => o.OrderId == (int)id);
        }

        public async Task<IEnumerable<Order>> GetUserOrderHistoryAsync(int userId, int pageNumber, int pageSize)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderLicensePlates)
                    .ThenInclude(olp => olp.LicensePlate)
                        .ThenInclude(lp => lp.Vehicle)
                            .ThenInclude(v => v.Brand)
                .Include(o => o.OrderLicensePlates)
                    .ThenInclude(olp => olp.LicensePlate)
                        .ThenInclude(lp => lp.Station)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUserOrderCountAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .CountAsync();
        }

        public async Task<IEnumerable<Order>> GetActiveOrdersByUserIdAsync(int userId)
        {
            var activeStatuses = new[] { "Pending", "Confirmed", "Paid", "Active", "Rented" };
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderLicensePlates)
                    .ThenInclude(olp => olp.LicensePlate)
                        .ThenInclude(lp => lp.Vehicle)
                            .ThenInclude(v => v.Brand)
                .Include(o => o.OrderLicensePlates)
                    .ThenInclude(olp => olp.LicensePlate)
                        .ThenInclude(lp => lp.Station)
                .Where(o => o.UserId == userId && activeStatuses.Contains(o.Status))
                .ToListAsync();
        }

        public async Task<bool> HasActiveOrderAsync(int userId)
        {
            var activeStatuses = new[] { "Pending", "Confirmed", "Paid", "Active", "Rented" };
            return await _context.Orders
                .AnyAsync(o => o.UserId == userId && activeStatuses.Contains(o.Status));
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderLicensePlates)
                    .ThenInclude(olp => olp.LicensePlate)
                        .ThenInclude(lp => lp.Vehicle)
                            .ThenInclude(v => v.Brand)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
        {
            var pendingStatuses = new[] { "Pending", "Pending Payment" };
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderLicensePlates)
                    .ThenInclude(olp => olp.LicensePlate)
                .Where(o => pendingStatuses.Contains(o.Status))
                .OrderBy(o => o.OrderDate)
                .ToListAsync();
        }
    }
}


