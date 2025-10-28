using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    /// <summary>
    /// Interface cho repository xử lý License
    /// </summary>
    public interface ILicenseRepository : IGenericRepository<License>
    {
        /// <summary>
        /// Lấy bằng lái xe theo ID
        /// </summary>
        /// <param name="id">ID của bằng lái xe</param>
        /// <returns>Bằng lái xe hoặc null</returns>
        Task<License?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy bằng lái xe theo số bằng
        /// </summary>
        /// <param name="licenseNumber">Số bằng lái xe</param>
        /// <returns>Bằng lái xe hoặc null</returns>
        Task<License?> GetByLicenseNumberAsync(string licenseNumber);

        /// <summary>
        /// Lấy tất cả bằng lái xe của user
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <returns>Danh sách bằng lái xe</returns>
        Task<IEnumerable<License>> GetByUserIdAsync(int userId);
    }
}
<<<<<<< HEAD
=======

>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
