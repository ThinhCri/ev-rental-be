using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // Business logic queries
        Task<IEnumerable<Order>> GetUserOrderHistoryAsync(int userId, int pageNumber, int pageSize);
        Task<int> GetUserOrderCountAsync(int userId);
        Task<IEnumerable<Order>> GetActiveOrdersByUserIdAsync(int userId);
        Task<bool> HasActiveOrderAsync(int userId);
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
        Task<IEnumerable<Order>> GetPendingOrdersAsync();
    }
}


