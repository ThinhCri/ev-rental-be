using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IContractRepository : IGenericRepository<Contract>
    {
        Task<IEnumerable<Contract>> GetContractsByOrderIdAsync(int orderId);
        Task<IEnumerable<Contract>> GetContractsByUserIdAsync(int userId);
        Task<Contract?> GetContractWithDetailsAsync(int contractId);
        Task<Contract?> GetContractByOrderIdAsync(int orderId);
    }
}
