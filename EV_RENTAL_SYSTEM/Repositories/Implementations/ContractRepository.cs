using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class ContractRepository : GenericRepository<Contract>, IContractRepository
    {
        public ContractRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Contract>> GetContractsByOrderIdAsync(int orderId)
        {
            return await _context.Contracts
                .Where(c => c.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetContractsByUserIdAsync(int userId)
        {
            return await _context.Contracts
                .Where(c => c.Order.UserId == userId)
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Payments)
                .ThenInclude(p => p.Transactions)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<Contract?> GetContractWithDetailsAsync(int contractId)
        {
            return await _context.Contracts
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .Include(c => c.Order)
                .ThenInclude(o => o.OrderLicensePlates)
                .ThenInclude(olp => olp.LicensePlate)
                .ThenInclude(lp => lp.Vehicle)
                .ThenInclude(v => v.Brand)
                .Include(c => c.Payments)
                .ThenInclude(p => p.Transactions)
                .FirstOrDefaultAsync(c => c.ContractId == contractId);
        }

        public async Task<Contract?> GetContractByOrderIdAsync(int orderId)
        {
            return await _context.Contracts
                .Include(c => c.Order)
                .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(c => c.OrderId == orderId);
        }
    }
}
