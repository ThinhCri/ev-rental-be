using EV_RENTAL_SYSTEM.Repositories.Interfaces;
<<<<<<< HEAD
=======
using Microsoft.EntityFrameworkCore.Storage;
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88

namespace EV_RENTAL_SYSTEM.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        ILicenseTypeRepository LicenseTypes { get; }
        ILicenseRepository Licenses { get; }
<<<<<<< HEAD
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
=======
        IVehicleRepository Vehicles { get; }
        IBrandRepository Brands { get; }
        IOrderRepository Orders { get; }
        IStationRepository Stations { get; }
        IPaymentRepository Payments { get; }
        ITransactionRepository Transactions { get; }
        ILicensePlateRepository LicensePlates { get; }
        IOrderLicensePlateRepository OrderLicensePlates { get; }
        IContractRepository Contracts { get; }
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
>>>>>>> 342ea64f1f50fa59204027b76484164ea0999d88
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

