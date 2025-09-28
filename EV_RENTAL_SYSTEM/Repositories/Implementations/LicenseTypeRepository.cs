using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    /// <summary>
    /// Repository xử lý LicenseType
    /// </summary>
    public class LicenseTypeRepository : GenericRepository<LicenseType>, ILicenseTypeRepository
    {
        public LicenseTypeRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy loại bằng lái xe theo ID
        /// </summary>
        /// <param name="id">ID của loại bằng lái xe</param>
        /// <returns>Loại bằng lái xe hoặc null</returns>
        public async Task<LicenseType?> GetByIdAsync(int id)
        {
            return await _context.LicenseTypes.FindAsync(id);
        }
    }
}

