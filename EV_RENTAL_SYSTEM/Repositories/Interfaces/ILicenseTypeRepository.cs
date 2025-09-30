using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    /// <summary>
    /// Interface cho repository xử lý LicenseType
    /// </summary>
    public interface ILicenseTypeRepository : IGenericRepository<LicenseType>
    {
        /// <summary>
        /// Lấy loại bằng lái xe theo ID
        /// </summary>
        /// <param name="id">ID của loại bằng lái xe</param>
        /// <returns>Loại bằng lái xe hoặc null</returns>
        Task<LicenseType?> GetByIdAsync(int id);
    }
}
