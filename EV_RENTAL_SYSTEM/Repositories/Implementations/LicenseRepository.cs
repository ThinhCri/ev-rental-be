using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    /// <summary>
    /// Repository xử lý License
    /// </summary>
    public class LicenseRepository : GenericRepository<License>, ILicenseRepository
    {
        public LicenseRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy bằng lái xe theo ID
        /// </summary>
        /// <param name="id">ID của bằng lái xe</param>
        /// <returns>Bằng lái xe hoặc null</returns>
        public async Task<License?> GetByIdAsync(int id)
        {
            return await _context.Licenses
                .Include(l => l.LicenseType)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LicenseId == id);
        }

        /// <summary>
        /// Lấy bằng lái xe theo số bằng
        /// </summary>
        /// <param name="licenseNumber">Số bằng lái xe</param>
        /// <returns>Bằng lái xe hoặc null</returns>
        public async Task<License?> GetByLicenseNumberAsync(string licenseNumber)
        {
            return await _context.Licenses
                .Include(l => l.LicenseType)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LicenseNumber == licenseNumber);
        }

        /// <summary>
        /// Lấy tất cả bằng lái xe của user
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <returns>Danh sách bằng lái xe</returns>
        public async Task<IEnumerable<License>> GetByUserIdAsync(int userId)
        {
            return await _context.Licenses
                .Include(l => l.LicenseType)
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }
    }
}

