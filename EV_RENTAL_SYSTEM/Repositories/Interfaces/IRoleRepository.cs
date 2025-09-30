using EV_RENTAL_SYSTEM.Models;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string roleName);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role> AddAsync(Role role);
        Task<Role> UpdateAsync(Role role);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

