using EV_RENTAL_SYSTEM.Repositories.Interfaces;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        ILicenseTypeRepository LicenseTypes { get; }
        ILicenseRepository Licenses { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

