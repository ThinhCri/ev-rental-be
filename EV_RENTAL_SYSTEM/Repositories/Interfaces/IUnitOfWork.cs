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
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

