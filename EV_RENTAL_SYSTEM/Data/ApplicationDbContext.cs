using EV_RENTAL_SYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Data
{
    /// <summary>
    /// Application Database Context
    /// 
    /// CÁCH HOẠT ĐỘNG:
    /// - Định nghĩa tất cả entities và relationships
    /// - Có data seeding trong OnModelCreating
    /// - Migration tự động chạy khi app start
    /// 
    /// CHO TEAM:
    /// - Khi thay đổi model, tạo migration: dotnet ef migrations add [Name]
    /// - Chạy migration: dotnet ef database update
    /// - Data seeding sẽ tự động chạy sau migration
    /// 
    /// ENTITIES:
    /// - User, Role, License, LicenseType
    /// - Vehicle, Brand, Station, LicensePlate
    /// - Order, Contract, Payment, Transaction
    /// - Complaint, Maintenance
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LicenseType> LicenseTypes { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<LicensePlate> LicensePlates { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_LicensePlate> OrderLicensePlates { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // Configure License entity
            modelBuilder.Entity<License>(entity =>
            {
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
            });

            // Configure relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<License>()
                .HasOne(l => l.User)
                .WithMany(u => u.Licenses)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<License>()
                .HasOne(l => l.LicenseType)
                .WithMany(lt => lt.Licenses)
                .HasForeignKey(l => l.LicenseTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle relationships
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Brand)
                .WithMany(b => b.Vehicles)
                .HasForeignKey(v => v.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // LicensePlate relationships
            modelBuilder.Entity<LicensePlate>()
                .HasOne(lp => lp.Vehicle)
                .WithMany(v => v.LicensePlates)
                .HasForeignKey(lp => lp.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LicensePlate>()
                .HasOne(lp => lp.Station)
                .WithMany(s => s.LicensePlates)
                .HasForeignKey(lp => lp.StationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order_LicensePlate relationships (many-to-many)
            modelBuilder.Entity<Order_LicensePlate>()
                .HasKey(ol => new { ol.OrderId, ol.LicensePlateId });

            modelBuilder.Entity<Order_LicensePlate>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLicensePlates)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order_LicensePlate>()
                .HasOne(ol => ol.LicensePlate)
                .WithMany(lp => lp.OrderLicensePlates)
                .HasForeignKey(ol => ol.LicensePlateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Complaint relationships
            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.User)
                .WithMany(u => u.Complaints)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Order)
                .WithMany(o => o.Complaints)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Maintenance relationships
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.LicensePlate)
                .WithMany(lp => lp.Maintenances)
                .HasForeignKey(m => m.LicensePlateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Contract relationships
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Order)
                .WithMany(o => o.Contracts)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment relationships
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Contract)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaction relationships
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Payment)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data for roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", Description = "System Administrator" },
                new Role { RoleId = 2, RoleName = "Station Staff", Description = "Station Staff Member" },
                new Role { RoleId = 3, RoleName = "EV Renter", Description = "Electric Vehicle Renter" }
            );

            // Seed data for license types
            modelBuilder.Entity<LicenseType>().HasData(
                new LicenseType { LicenseTypeId = "A1", TypeName = "A1", Description = "Motorcycle up to 125cc" },
                new LicenseType { LicenseTypeId = "A2", TypeName = "A2", Description = "Motorcycle up to 175cc" },
                new LicenseType { LicenseTypeId = "A", TypeName = "A", Description = "Unlimited motorcycle" },
                new LicenseType { LicenseTypeId = "B1", TypeName = "B1", Description = "Car up to 9 seats" },
                new LicenseType { LicenseTypeId = "B2", TypeName = "B2", Description = "Unlimited car" }
            );
        }
    }
}
