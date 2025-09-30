using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        ILicenseTypeRepository LicenseTypes { get; }
        ILicenseRepository Licenses { get; }
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

