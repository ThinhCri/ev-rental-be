using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class ComplaintRepository : GenericRepository<Complaint>, IComplaintRepository
    {
        public ComplaintRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsByUserIdAsync(int userId)
        {
            return await Task.FromResult(_context.Set<Complaint>()
                .Where(c => c.UserId == userId)
                .ToList());
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsByOrderIdAsync(int orderId)
        {
            return await Task.FromResult(_context.Set<Complaint>()
                .Where(c => c.OrderId == orderId)
                .ToList());
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsByStatusAsync(string status)
        {
            return await Task.FromResult(_context.Set<Complaint>()
                .Where(c => c.Status == status)
                .ToList());
        }
    }
}

