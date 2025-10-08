using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    /// <summary>
    /// Data Seeding Service - Automatically creates sample data for the application
    /// 
    /// HOW IT WORKS:
    /// - Runs automatically when app starts
    /// - Only adds data if not already exists (safe)
    /// - Creates consistent data for all team members
    /// 
    /// FOR TEAM:
    /// - When cloning code, just run: dotnet run
    /// - Data will be created automatically, no additional setup needed
    /// - To add new data, modify this file and commit
    /// 
    /// DATA CREATED:
    /// - Roles: Admin, Station Staff, EV Renter
    /// - License Types: A1, A2, A, B1, B2
    /// - Brands: Honda, Yamaha, Tesla, BMW, Mercedes, etc.
    /// - Stations: 4 stations in HCM
    /// - Process Steps: 10 contract processing steps
    /// - Sample Vehicles: Electric motorcycles and cars
    /// - Sample License Plates: Sample license plates
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
            await SeedAdminUsersAsync();
            await SeedStaffUsersAsync();
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
                    // Ho Chi Minh City Stations
                    new Station 
                    { 
                        StationName = "HCM Central Station", 
                        Street = "123 Nguyen Hue Boulevard", 
                        District = "District 1", 
                        Province = "Ho Chi Minh City", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "HCM Business District Station", 
                        Street = "456 Le Loi Street", 
                        District = "District 3", 
                        Province = "Ho Chi Minh City", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "HCM Binh Thanh Station", 
                        Street = "789 Cach Mang Thang 8 Street", 
                        District = "Binh Thanh", 
                        Province = "Ho Chi Minh City", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "HCM New Urban Station", 
                        Street = "321 Nguyen Van Linh Boulevard", 
                        District = "District 7", 
                        Province = "Ho Chi Minh City", 
                        Country = "Vietnam" 
                    },
                    // Binh Duong Stations
                    new Station 
                    { 
                        StationName = "BD Industrial Station", 
                        Street = "123 Nguyen Thi Minh Khai Street", 
                        District = "Thu Dau Mot", 
                        Province = "Binh Duong", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "BD Gateway Station", 
                        Street = "456 National Highway 1A", 
                        District = "Di An", 
                        Province = "Binh Duong", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "BD Tech Park Station", 
                        Street = "789 Provincial Road 743", 
                        District = "Tan Uyen", 
                        Province = "Binh Duong", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "BD Manufacturing Station", 
                        Street = "321 National Highway 13", 
                        District = "Ben Cat", 
                        Province = "Binh Duong", 
                        Country = "Vietnam" 
                    },
                    // Da Nang Stations
                    new Station 
                    { 
                        StationName = "DN City Center Station", 
                        Street = "123 Le Duan Street", 
                        District = "Hai Chau", 
                        Province = "Da Nang", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "DN Airport Station", 
                        Street = "456 Dien Bien Phu Street", 
                        District = "Thanh Khe", 
                        Province = "Da Nang", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "DN Beach Station", 
                        Street = "789 Vo Nguyen Giap Street", 
                        District = "Son Tra", 
                        Province = "Da Nang", 
                        Country = "Vietnam" 
                    },
                    new Station 
                    { 
                        StationName = "DN University Station", 
                        Street = "321 Nguyen Van Linh Street", 
                        District = "Cam Le", 
                        Province = "Da Nang", 
                        Country = "Vietnam" 
                    }
                };

                _context.Stations.AddRange(stations);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Stations seeded successfully (HCM, BD, DN)");
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
                var bmwBrand = await _context.Brands.FirstAsync(b => b.BrandName == "BMW");
                var mercedesBrand = await _context.Brands.FirstAsync(b => b.BrandName == "Mercedes");
                var suzukiBrand = await _context.Brands.FirstAsync(b => b.BrandName == "Suzuki");

                var stations = await _context.Stations.ToListAsync();
                var hcmStations = stations.Where(s => s.Province == "Ho Chi Minh City").ToList();
                var bdStations = stations.Where(s => s.Province == "Binh Duong").ToList();
                var dnStations = stations.Where(s => s.Province == "Da Nang").ToList();

                var vehicles = new List<Vehicle>();

                // Ho Chi Minh City Vehicles (10 vehicles)
                for (int i = 1; i <= 10; i++)
                {
                    var station = hcmStations[(i - 1) % hcmStations.Count];
					var brand = i % 3 == 0 ? teslaBrand : (i % 2 == 0 ? hondaBrand : yamahaBrand);
					var isCar = true;
                    
                    vehicles.Add(new Vehicle 
                    { 
						Model = $"Tesla Model {(i % 2 == 0 ? "3" : "Y")}",
                        ModelYear = 2024, 
                        BrandId = brand.BrandId, 
						Description = $"Premium electric vehicle with autopilot - HCM Branch",
						PricePerDay = 2500000, 
						SeatNumber = 5,
						Battery = isCar ? 75.0m : 2.5m,
                        RangeKm = isCar ? 500 : 50,
                        Status = "Good",
                        StationId = station.StationId
                    });
                }

                // Binh Duong Vehicles (10 vehicles)
                for (int i = 1; i <= 10; i++)
                {
                    var station = bdStations[(i - 1) % bdStations.Count];
					var brand = i % 3 == 0 ? bmwBrand : (i % 2 == 0 ? suzukiBrand : yamahaBrand);
					var isCar = true;
                    
                    vehicles.Add(new Vehicle 
                    { 
						Model = $"BMW iX{i}",
                        ModelYear = 2024, 
                        BrandId = brand.BrandId, 
						Description = $"Luxury electric SUV - BD Branch",
						PricePerDay = 3000000, 
						SeatNumber = 5,
						Battery = isCar ? 80.0m : 2.0m,
                        RangeKm = isCar ? 600 : 45,
                        Status = "Good",
                        StationId = station.StationId
                    });
                }

                // Da Nang Vehicles (10 vehicles)
                for (int i = 1; i <= 10; i++)
                {
                    var station = dnStations[(i - 1) % dnStations.Count];
					var brand = i % 3 == 0 ? mercedesBrand : (i % 2 == 0 ? hondaBrand : suzukiBrand);
					var isCar = true;
                    
                    vehicles.Add(new Vehicle 
                    { 
						Model = $"Mercedes EQS {i}",
                        ModelYear = 2024, 
                        BrandId = brand.BrandId, 
						Description = $"Luxury electric sedan - DN Branch",
						PricePerDay = 3500000, 
						SeatNumber = 5,
						Battery = isCar ? 90.0m : 3.0m,
                        RangeKm = isCar ? 700 : 60,
                        Status = "Good",
                        StationId = station.StationId
                    });
                }

                _context.Vehicles.AddRange(vehicles);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Sample Vehicles seeded successfully (30 vehicles across 3 branches)");
            }
        }

        public async Task SeedSampleLicensePlatesAsync()
        {
            if (!await _context.LicensePlates.AnyAsync())
            {
                var vehicles = await _context.Vehicles.ToListAsync();
                var stations = await _context.Stations.ToListAsync();

                var licensePlates = new List<LicensePlate>();
                var random = new Random();

                // Generate license plates for all 30 vehicles
                for (int i = 0; i < vehicles.Count; i++)
                {
                    var vehicle = vehicles[i];
                    var station = stations.First(s => s.StationId == vehicle.StationId);
                    
                    // Generate license plate based on province
                    string provinceCode = station.Province switch
                    {
                        "Ho Chi Minh City" => "51A",
                        "Binh Duong" => "61A", 
                        "Da Nang" => "43A",
                        _ => "51A"
                    };

                    var plateNumber = $"{provinceCode}-{10000 + i:00000}";
                    var statuses = new[] { "Available", "Available", "Available", "Maintenance", "Rented" };
                    var conditions = new[] { "Excellent", "Good", "Good", "Fair" };
                    
       licensePlates.Add(new LicensePlate
       {
           LicensePlateId = plateNumber, // Sử dụng LicensePlateId để lưu biển số
           Status = statuses[random.Next(statuses.Length)],
           VehicleId = vehicle.VehicleId,
           Province = station.Province,
           RegistrationDate = DateTime.Now.AddDays(-random.Next(1, 90)),
           Condition = conditions[random.Next(conditions.Length)],
           StationId = station.StationId
       });
                }

                _context.LicensePlates.AddRange(licensePlates);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Sample License Plates seeded successfully (30 plates)");
            }
        }

        /// <summary>
        /// Tạo tài khoản admin mẫu
        /// </summary>
        public async Task SeedAdminUsersAsync()
        {
            if (!await _context.Users.AnyAsync(u => u.Email == "admin@evrental.com"))
            {
                // Lấy role Admin
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
                if (adminRole == null)
                {
                    Console.WriteLine("❌ Admin role not found. Please run SeedRolesAsync first.");
                    return;
                }

                var adminUsers = new List<User>
                {
                    new User
                    {
                        FullName = "System Administrator",
                        Email = "admin@evrental.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        PhoneNumber = "0123456789",
                        Birthday = new DateTime(1990, 1, 1),
                        Status = "Active",
                        RoleId = adminRole.RoleId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Station Manager",
                        Email = "manager@evrental.com", 
                        Password = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                        PhoneNumber = "0987654321",
                        Birthday = new DateTime(1985, 5, 15),
                        Status = "Active",
                        RoleId = adminRole.RoleId,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                _context.Users.AddRange(adminUsers);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Admin users seeded successfully");
                Console.WriteLine("📧 Admin Email: admin@evrental.com | Password: Admin123!");
                Console.WriteLine("📧 Manager Email: manager@evrental.com | Password: Manager123!");
            }
        }

        /// <summary>
        /// Tạo tài khoản staff mẫu
        /// </summary>
        public async Task SeedStaffUsersAsync()
        {
            if (!await _context.Users.AnyAsync(u => u.Email == "staff1@evrental.com"))
            {
                // Lấy role Station Staff
                var staffRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Station Staff");
                if (staffRole == null)
                {
                    Console.WriteLine("❌ Station Staff role not found. Please run SeedRolesAsync first.");
                    return;
                }

                var staffUsers = new List<User>
                {
                    new User
                    {
                        FullName = "Nguyễn Văn A",
                        Email = "staff1@evrental.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Staff123!"),
                        PhoneNumber = "0123456780",
                        Birthday = new DateTime(1992, 3, 15),
                        Status = "Active",
                        RoleId = staffRole.RoleId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Trần Thị B",
                        Email = "staff2@evrental.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Staff123!"),
                        PhoneNumber = "0123456781",
                        Birthday = new DateTime(1988, 7, 22),
                        Status = "Active",
                        RoleId = staffRole.RoleId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Lê Văn C",
                        Email = "staff3@evrental.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Staff123!"),
                        PhoneNumber = "0123456782",
                        Birthday = new DateTime(1995, 11, 8),
                        Status = "Active",
                        RoleId = staffRole.RoleId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Phạm Thị D",
                        Email = "staff4@evrental.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Staff123!"),
                        PhoneNumber = "0123456783",
                        Birthday = new DateTime(1990, 9, 12),
                        Status = "Active",
                        RoleId = staffRole.RoleId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Hoàng Văn E",
                        Email = "staff5@evrental.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("Staff123!"),
                        PhoneNumber = "0123456784",
                        Birthday = new DateTime(1987, 4, 25),
                        Status = "Active",
                        RoleId = staffRole.RoleId,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                _context.Users.AddRange(staffUsers);
                await _context.SaveChangesAsync();
                Console.WriteLine("✓ Staff users seeded successfully");
                Console.WriteLine("📧 Staff 1 Email: staff1@evrental.com | Password: Staff123!");
                Console.WriteLine("📧 Staff 2 Email: staff2@evrental.com | Password: Staff123!");
                Console.WriteLine("📧 Staff 3 Email: staff3@evrental.com | Password: Staff123!");
                Console.WriteLine("📧 Staff 4 Email: staff4@evrental.com | Password: Staff123!");
                Console.WriteLine("📧 Staff 5 Email: staff5@evrental.com | Password: Staff123!");
            }
        }
    }
}
