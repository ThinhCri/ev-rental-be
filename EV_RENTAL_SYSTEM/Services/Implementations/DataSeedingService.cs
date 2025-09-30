using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    /// <summary>
    /// Data Seeding Service - Tự động tạo data mẫu cho ứng dụng
    /// 
    /// CÁCH HOẠT ĐỘNG:
    /// - Chạy tự động khi app khởi động
    /// - Chỉ thêm data nếu chưa có (an toàn)
    /// - Tạo data nhất quán cho tất cả team members
    /// 
    /// CHO TEAM:
    /// - Khi clone code về, chỉ cần chạy: dotnet run
    /// - Data sẽ tự động được tạo, không cần làm gì thêm
    /// - Nếu muốn thêm data mới, sửa file này và commit
    /// 
    /// DATA ĐƯỢC TẠO:
    /// - Roles: Admin, Station Staff, EV Renter
    /// - License Types: A1, A2, A, B1, B2
    /// - Brands: Honda, Yamaha, Tesla, BMW, Mercedes, etc.
    /// - Stations: 4 stations ở HCM
    /// - Process Steps: 10 bước xử lý contract
    /// - Sample Vehicles: Xe máy điện và ô tô điện
    /// - Sample License Plates: Biển số mẫu
    /// </summary>
    public class DataSeedingService : IDataSeedingService
    {
        private readonly ApplicationDbContext _context;

        public DataSeedingService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Chạy tất cả data seeding methods
        /// Gọi tự động khi app khởi động
        /// </summary>
        public async Task SeedDataAsync()
        {
            await SeedRolesAsync();
            await SeedLicenseTypesAsync();
            await SeedBrandsAsync();
            await SeedStationsAsync();
            await SeedProcessStepsAsync();
            await SeedSampleVehiclesAsync();
            await SeedSampleLicensePlatesAsync();
        }

        public async Task SeedRolesAsync()
        {
            if (!await _context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { RoleId = 1, RoleName = "Admin", Description = "System Administrator" },
                    new Role { RoleId = 2, RoleName = "Station Staff", Description = "Station Staff Member" },
                    new Role { RoleId = 3, RoleName = "EV Renter", Description = "Electric Vehicle Renter" }
                };

                _context.Roles.AddRange(roles);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Roles seeded successfully");
            }
        }

        public async Task SeedLicenseTypesAsync()
        {
            if (!await _context.LicenseTypes.AnyAsync())
            {
                var licenseTypes = new List<LicenseType>
                {
                    new LicenseType { LicenseTypeId = "A1", TypeName = "A1", Description = "Motorcycle up to 125cc" },
                    new LicenseType { LicenseTypeId = "A2", TypeName = "A2", Description = "Motorcycle up to 175cc" },
                    new LicenseType { LicenseTypeId = "A", TypeName = "A", Description = "Unlimited motorcycle" },
                    new LicenseType { LicenseTypeId = "B1", TypeName = "B1", Description = "Car up to 9 seats" },
                    new LicenseType { LicenseTypeId = "B2", TypeName = "B2", Description = "Unlimited car" }
                };

                _context.LicenseTypes.AddRange(licenseTypes);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ License Types seeded successfully");
            }
        }

        public async Task SeedBrandsAsync()
        {
            if (!await _context.Brands.AnyAsync())
            {
                var brands = new List<Brand>
                {
                    new Brand { BrandName = "Honda" },
                    new Brand { BrandName = "Yamaha" },
                    new Brand { BrandName = "Suzuki" },
                    new Brand { BrandName = "Kawasaki" },
                    new Brand { BrandName = "Ducati" },
                    new Brand { BrandName = "Tesla" },
                    new Brand { BrandName = "BMW" },
                    new Brand { BrandName = "Mercedes" }
                };

                _context.Brands.AddRange(brands);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Brands seeded successfully");
            }
        }

        public async Task SeedStationsAsync()
        {
            if (!await _context.Stations.AnyAsync())
            {
                var stations = new List<Station>
                {
                    new Station 
                    { 
                        StationName = "Station 1 - District 1", 
                        Street = "123 Main Street", 
                        District = "District 1", 
                        Province = "Ho Chi Minh City", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "Station 2 - District 3", 
                        Street = "456 Le Loi", 
                        District = "District 3", 
                        Province = "Ho Chi Minh City", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "Station 3 - District 1", 
                        Street = "789 Nguyen Hue", 
                        District = "District 1", 
                        Province = "Ho Chi Minh City", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "Station 4 - Binh Thanh", 
                        Street = "321 Cach Mang Thang 8", 
                        District = "Binh Thanh", 
                        Province = "Ho Chi Minh City", 
                        Country = "Vietnam" 
                    }
                };

                _context.Stations.AddRange(stations);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Stations seeded successfully");
            }
        }

        public async Task SeedProcessStepsAsync()
        {
            if (!await _context.ProcessSteps.AnyAsync())
            {
                var processSteps = new List<ProcessStep>
                {
                    new ProcessStep { StepName = "Application Review", Terms = "Review customer application and documents" },
                    new ProcessStep { StepName = "License Verification", Terms = "Verify driving license and identity" },
                    new ProcessStep { StepName = "Payment Processing", Terms = "Process rental payment and deposit" },
                    new ProcessStep { StepName = "Vehicle Assignment", Terms = "Assign vehicle to customer" },
                    new ProcessStep { StepName = "Contract Signing", Terms = "Sign rental contract" },
                    new ProcessStep { StepName = "Vehicle Handover", Terms = "Hand over vehicle to customer" },
                    new ProcessStep { StepName = "Rental Monitoring", Terms = "Monitor rental period" },
                    new ProcessStep { StepName = "Vehicle Return", Terms = "Process vehicle return" },
                    new ProcessStep { StepName = "Final Inspection", Terms = "Inspect vehicle condition" },
                    new ProcessStep { StepName = "Payment Settlement", Terms = "Settle final payment" }
                };

                _context.ProcessSteps.AddRange(processSteps);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Process Steps seeded successfully");
            }
        }

        public async Task SeedSampleVehiclesAsync()
        {
            if (!await _context.Vehicles.AnyAsync())
            {
                var hondaBrand = await _context.Brands.FirstAsync(b => b.BrandName == "Honda");
                var yamahaBrand = await _context.Brands.FirstAsync(b => b.BrandName == "Yamaha");
                var teslaBrand = await _context.Brands.FirstAsync(b => b.BrandName == "Tesla");

                var vehicles = new List<Vehicle>
                {
                    // Electric Motorcycles
                    new Vehicle 
                    { 
                        Model = "Honda PCX Electric", 
                        ModelYear = 2024, 
                        BrandId = hondaBrand.BrandId, 
                        VehicleType = "Electric Motorcycle", 
                        Description = "Electric scooter with 50km range", 
                        DailyRate = 150000, 
                        SeatNumber = 2 
                    },
                    new Vehicle 
                    { 
                        Model = "Yamaha E-Vino", 
                        ModelYear = 2024, 
                        BrandId = yamahaBrand.BrandId, 
                        VehicleType = "Electric Motorcycle", 
                        Description = "Compact electric scooter", 
                        DailyRate = 120000, 
                        SeatNumber = 2 
                    },
                    // Electric Cars
                    new Vehicle 
                    { 
                        Model = "Tesla Model 3", 
                        ModelYear = 2024, 
                        BrandId = teslaBrand.BrandId, 
                        VehicleType = "Electric Car", 
                        Description = "Premium electric sedan with autopilot", 
                        DailyRate = 2500000, 
                        SeatNumber = 5 
                    },
                    new Vehicle 
                    { 
                        Model = "Tesla Model Y", 
                        ModelYear = 2024, 
                        BrandId = teslaBrand.BrandId, 
                        VehicleType = "Electric SUV", 
                        Description = "Electric SUV with 7 seats", 
                        DailyRate = 3000000, 
                        SeatNumber = 7 
                    }
                };

                _context.Vehicles.AddRange(vehicles);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Sample Vehicles seeded successfully");
            }
        }

        public async Task SeedSampleLicensePlatesAsync()
        {
            if (!await _context.LicensePlates.AnyAsync())
            {
                var vehicles = await _context.Vehicles.ToListAsync();
                var stations = await _context.Stations.ToListAsync();

                var licensePlates = new List<LicensePlate>
                {
                    new LicensePlate 
                    { 
                        LicensePlateId = "51A-12345", 
                        Status = "Available", 
                        VehicleId = vehicles[0].VehicleId, 
                        Province = "Ho Chi Minh City", 
                        RegistrationDate = DateTime.Now.AddDays(-30), 
                        Condition = "Excellent", 
                        StationId = stations[0].StationId 
                    },
                    new LicensePlate 
                    { 
                        LicensePlateId = "51A-67890", 
                        Status = "Available", 
                        VehicleId = vehicles[1].VehicleId, 
                        Province = "Ho Chi Minh City", 
                        RegistrationDate = DateTime.Now.AddDays(-25), 
                        Condition = "Good", 
                        StationId = stations[1].StationId 
                    },
                    new LicensePlate 
                    { 
                        LicensePlateId = "51A-11111", 
                        Status = "Available", 
                        VehicleId = vehicles[2].VehicleId, 
                        Province = "Ho Chi Minh City", 
                        RegistrationDate = DateTime.Now.AddDays(-20), 
                        Condition = "Excellent", 
                        StationId = stations[0].StationId 
                    },
                    new LicensePlate 
                    { 
                        LicensePlateId = "51A-22222", 
                        Status = "Maintenance", 
                        VehicleId = vehicles[3].VehicleId, 
                        Province = "Ho Chi Minh City", 
                        RegistrationDate = DateTime.Now.AddDays(-15), 
                        Condition = "Good", 
                        StationId = stations[2].StationId 
                    }
                };

                _context.LicensePlates.AddRange(licensePlates);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Sample License Plates seeded successfully");
            }
        }
    }
}
