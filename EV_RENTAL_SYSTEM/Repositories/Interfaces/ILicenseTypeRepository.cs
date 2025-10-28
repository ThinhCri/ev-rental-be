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
<<<<<<< HEAD
    }
}
=======

        /// <summary>
        /// Lấy loại bằng lái xe theo tên
        /// </summary>
        /// <param name="name">Tên loại bằng lái xe</param>
        /// <returns>Loại bằng lái xe hoặc null</returns>
        Task<LicenseType?> GetByNameAsync(string name);
    }
}

>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
