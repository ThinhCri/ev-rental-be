using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Payment)
                .ThenInclude(p => p.Contract)
                .ThenInclude(c => c.Order)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByTransactionIdAsync(string transactionId)
        {
            if (int.TryParse(transactionId, out int id))
            {
                return await _context.Transactions
                    .Include(t => t.Payment)
                    .ThenInclude(p => p.Contract)
                    .ThenInclude(c => c.Order)
                    .ThenInclude(o => o.User)
                    .FirstOrDefaultAsync(t => t.TransactionId == id);
            }
            return null;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByPaymentIdAsync(int paymentId)
        {
            return await _context.Transactions
                .Where(t => t.PaymentId == paymentId)
                .Include(t => t.User)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
    }
}

