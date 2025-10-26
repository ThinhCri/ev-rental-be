using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace EV_RENTAL_SYSTEM.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Roles = new RoleRepository(_context);
            LicenseTypes = new LicenseTypeRepository(_context);
            Licenses = new LicenseRepository(_context);
            Vehicles = new VehicleRepository(_context);
            Brands = new BrandRepository(_context);
            Orders = new OrderRepository(_context);
            Stations = new StationRepository(_context);
            Payments = new PaymentRepository(_context);
            Transactions = new TransactionRepository(_context);
            LicensePlates = new LicensePlateRepository(_context);
            OrderLicensePlates = new OrderLicensePlateRepository(_context);
            Contracts = new ContractRepository(_context);
            Maintenances = new MaintenanceRepository(_context);
        }

        public IUserRepository Users { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public ILicenseTypeRepository LicenseTypes { get; private set; }
        public ILicenseRepository Licenses { get; private set; }
        public IVehicleRepository Vehicles { get; private set; }
        public IBrandRepository Brands { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IStationRepository Stations { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public ITransactionRepository Transactions { get; private set; }
        public ILicensePlateRepository LicensePlates { get; private set; }
        public IOrderLicensePlateRepository OrderLicensePlates { get; private set; }
        public IContractRepository Contracts { get; private set; }
        public IMaintenanceRepository Maintenances { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

