using EV_RENTAL_SYSTEM.Attributes;
using EV_RENTAL_SYSTEM.Data;
using EV_RENTAL_SYSTEM.Mappings;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Implementations;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Implementations;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ILicenseTypeRepository, LicenseTypeRepository>();
builder.Services.AddScoped<ILicenseRepository, LicenseRepository>();
builder.Services.AddScoped<ILicensePlateRepository, LicensePlateRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IStationRepository, StationRepository>();

// Cloudinary Configuration
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<ICloudService, CloudinaryService>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IDataSeedingService, DataSeedingService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IRevenueService, RevenueService>();

// Custom JWT Authentication (không cần Bearer prefix)
builder.Services.AddAuthentication("CustomJwt")
    .AddScheme<AuthenticationSchemeOptions, CustomJwtAuthenticationHandler>("CustomJwt", options => { });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOrAdmin", policy => policy.RequireRole("Admin", "Station Staff", "Staff"));
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        // Cấu hình định dạng ngày tháng
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "EV Rental System API", 
        Version = "v1",
        Description = "API for Electric Vehicle Rental System"
    });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: \"Authorization: {token}\" (no Bearer prefix needed)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "JWT"
                }
            },
            new string[] {}
        }
    });
    
    // Cấu hình để hiển thị đúng định dạng DateTime
    c.MapType<DateTime>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "date",
        Example = new Microsoft.OpenApi.Any.OpenApiString("1990-01-15")
    });
    
    c.MapType<DateTime?>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "date",
        Example = new Microsoft.OpenApi.Any.OpenApiString("1990-01-15")
    });
    
    
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EV Rental System API V1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve static files (ảnh upload)

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/", () => "EV Rental System API is running!");
app.MapGet("/health", () => "OK");

// ========================================
// DATABASE SETUP & DATA SEEDING
// ========================================
// 
// CÁCH HOẠT ĐỘNG:
// 1. Kiểm tra database có tồn tại không
// 2. Nếu có: chạy migration (nếu cần)
// 3. Nếu không: tạo database mới
// 4. Chạy data seeding để tạo data mẫu
// 
// CHO TEAM:
// - Khi clone code về, chỉ cần chạy: dotnet run
// - Data sẽ tự động được tạo (Roles, LicenseTypes, Brands, etc.)
// - Không cần xóa migration cũ, chỉ cần chạy migration
// - Data seeding an toàn - chỉ thêm data chưa có
//
// ========================================

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var dataSeedingService = scope.ServiceProvider.GetRequiredService<IDataSeedingService>();
    
    try
    {
        // Kiểm tra xem database có tồn tại không
        if (context.Database.CanConnect())
        {
            // Database đã tồn tại, chỉ migrate nếu cần
            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Found {pendingMigrations.Count()} pending migrations. Applying...");
                context.Database.Migrate();
                Console.WriteLine("✓ Database migration completed successfully.");
            }
            else
            {
                Console.WriteLine("✓ Database is up to date.");
            }
        }
        else
        {
            // Database chưa tồn tại, tạo mới
            Console.WriteLine("Creating new database...");
            context.Database.EnsureCreated();
            Console.WriteLine("✓ Database created successfully.");
        }

        // ========================================
        // DATA SEEDING - TỰ ĐỘNG TẠO DATA MẪU
        // ========================================
        // Data seeding sẽ tạo:
        // - 3 Roles (Admin, Station Staff, EV Renter)
        // - 5 License Types (A1, A2, A, B1, B2)
        // - 8 Brands (Honda, Yamaha, Tesla, etc.)
        // - 4 Stations (ở HCM)
        // - 10 Process Steps (xử lý contract)
        // - 4 Sample Vehicles (xe mẫu)
        // - 4 Sample License Plates (biển số mẫu)
        // ========================================
        Console.WriteLine("Starting data seeding...");
        await dataSeedingService.SeedDataAsync();
        Console.WriteLine("✓ Data seeding completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error during database setup: {ex.Message}");
        // Fallback to EnsureCreated if migration fails
        try
        {
            context.Database.EnsureCreated();
            Console.WriteLine("✓ Database created using EnsureCreated fallback.");
            
            // Try to seed data even with fallback
            await dataSeedingService.SeedDataAsync();
            Console.WriteLine("✓ Data seeding completed with fallback.");
        }
        catch (Exception fallbackEx)
        {
            Console.WriteLine($"❌ Fallback also failed: {fallbackEx.Message}");
        }
    }
}

app.Run();
