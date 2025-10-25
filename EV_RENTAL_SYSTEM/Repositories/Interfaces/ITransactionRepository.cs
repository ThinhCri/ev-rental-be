using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId);
        Task<Transaction?> GetTransactionByTransactionIdAsync(string transactionId);
        Task<IEnumerable<Transaction>> GetTransactionsByPaymentIdAsync(int paymentId);
    }
}

