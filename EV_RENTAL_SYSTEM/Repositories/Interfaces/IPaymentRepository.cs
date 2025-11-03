using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId);
        Task<IEnumerable<Payment>> GetPaymentsByContractIdAsync(int contractId);
        Task<Payment?> GetPaymentByContractIdAsync(int contractId);
        Task<Payment?> GetPaymentByTransactionIdAsync(string transactionId);
        Task<IEnumerable<Payment>> GetPaymentsWithPaginationAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Payment>> GetPaymentsByStationIdWithPaginationAsync(int stationId, int pageNumber, int pageSize);
        Task<int> GetTotalPaymentCountAsync();
        Task<int> GetTotalPaymentCountByStationIdAsync(int stationId);
    }
}

