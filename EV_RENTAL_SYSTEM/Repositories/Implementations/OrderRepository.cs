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
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderLicensePlates)
                    .ThenInclude(olp => olp.LicensePlate)
                        .ThenInclude(lp => lp.Vehicle)
                            .ThenInclude(v => v.Brand)
                .Include(o => o.OrderLicensePlates)
                    .ThenInclude(olp => olp.LicensePlate)
                        .ThenInclude(lp => lp.Station)
                .Where(o => o.UserId == userId && (o.Status == "Active" || o.Status == "Rented"))
                .ToListAsync();
        }

        public async Task<bool> HasActiveOrderAsync(int userId)
        {
            return await _context.Orders
                .AnyAsync(o => o.UserId == userId && (o.Status == "Active" || o.Status == "Rented"));
        }
    }
}


