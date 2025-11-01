using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IComplaintRepository : IGenericRepository<Complaint>
    {
        Task<IEnumerable<Complaint>> GetComplaintsByUserIdAsync(int userId);
        Task<IEnumerable<Complaint>> GetComplaintsByOrderIdAsync(int orderId);
        Task<IEnumerable<Complaint>> GetComplaintsByStatusAsync(string status);
    }
}

