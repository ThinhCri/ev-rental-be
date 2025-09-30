# EV Rental System API

A .NET 8 Web API for Electric Vehicle Rental System with JWT Authentication.

## Features

- User Registration and Login
- JWT Token Authentication
- Role-based Authorization (Admin, Station Staff, EV Renter)
- Clean Architecture with Repository Pattern
- Entity Framework Core with SQL Server
- AutoMapper for DTOs
- Swagger Documentation

## Project Structure

```
EV_RENTAL_SYSTEM/
├── Controllers/           # API Controllers
│   └── AuthController.cs
├── Data/                  # Database Context
│   └── ApplicationDbContext.cs
├── Models/                # Entity Models and DTOs
│   ├── DTOs/
│   │   └── AuthRequestDto.cs
│   ├── User.cs
│   ├── Role.cs
│   ├── License.cs
│   └── LicenseType.cs
├── Repositories/          # Repository Pattern
│   ├── Interfaces/
│   │   ├── IUserRepository.cs
│   │   ├── IRoleRepository.cs
│   │   └── IUnitOfWork.cs
│   └── Implementations/
│       ├── UserRepository.cs
│       ├── RoleRepository.cs
│       └── UnitOfWork.cs
├── Services/              # Business Logic
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   └── IJwtService.cs
│   └── Implementations/
│       ├── AuthService.cs
│       └── JwtService.cs
├── Mappings/              # AutoMapper Profiles
│   └── AutoMapperProfile.cs
├── Program.cs             # Application Configuration
└── appsettings.json       # Configuration Settings
```

## API Endpoints

### Authentication

#### POST /api/auth/register
Register a new user account.

**Request Body:**
```json
{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "password123",
  "confirmPassword": "password123",
  "birthday": "1990-01-01"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Registration successful.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "userId": 1,
    "fullName": "John Doe",
    "email": "john@example.com",
    "birthday": "1990-01-01T00:00:00",
    "createdAt": "2024-01-01T00:00:00",
    "status": "Active",
    "roleName": "EV Renter"
  }
}
```

#### POST /api/auth/login
Login with email and password.

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "userId": 1,
    "fullName": "John Doe",
    "email": "john@example.com",
    "birthday": "1990-01-01T00:00:00",
    "createdAt": "2024-01-01T00:00:00",
    "status": "Active",
    "roleName": "EV Renter"
  }
}
```

#### POST /api/auth/logout
Logout (requires authentication).

**Headers:**
```
Authorization: Bearer {token}
```

**Response:**
```json
{
  "message": "Logout successful"
}
```

#### GET /api/auth/validate
Validate JWT token (requires authentication).

**Headers:**
```
Authorization: Bearer {token}
```

**Response:**
```json
{
  "message": "Token is valid"
}
```

## Configuration

### Database Connection
Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EV_Rental_System;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### JWT Settings
Configure JWT settings in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "EV_Rental_System",
    "Audience": "EV_Rental_System_Users",
    "ExpiryInMinutes": 60
  }
}
```

## 🚀 QUICK START - CHO TEAM MEMBERS

### ⚡ Setup Tự Động (Khuyến nghị)

**Chỉ cần chạy 1 lệnh duy nhất:**

```powershell
.\setup.ps1
```

Script này sẽ tự động:
- ✅ Restore packages
- ✅ Build project  
- ✅ Xử lý database (xóa cũ, tạo mới)
- ✅ Chạy migration
- ✅ Data seeding (tạo data mẫu)
- ✅ Kiểm tra kết quả

### 📋 Prerequisites

Trước khi chạy, đảm bảo đã cài đặt:

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** - [Download SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **PowerShell** (có sẵn trên Windows)

### 🔧 Setup Thủ Công (Nếu cần)

1. **Clone Repository**
   ```bash
   git clone <your-github-repo-url>
   cd EV_RENTAL_SYSTEM
   ```

2. **Chạy Setup Script**
   ```powershell
   .\setup.ps1
   ```

3. **Hoặc Setup Thủ Công:**
   ```bash
   dotnet restore
   dotnet build
   dotnet ef database drop --force
   dotnet ef database update
   dotnet run
   ```

### 🌐 Truy Cập Ứng Dụng

- **Swagger UI:** http://localhost:5228/swagger
- **API Base URL:** http://localhost:5228/api
- **Lưu ý:** Sử dụng HTTP (port 5228), không phải HTTPS

### First Run

When you run the application for the first time:

1. The application will automatically:
   - Create the database if it doesn't exist
   - Run any pending migrations
   - Seed initial data (roles, license types)

2. You should see console output like:
   ```
   Database is up to date.
   Now listening on: https://localhost:7181
   Now listening on: http://localhost:5228
   Application started. Press Ctrl+C to shut down.
   ```

3. If you see any database errors, make sure:
   - SQL Server is running
   - Connection string is correct
   - You have permission to create databases

### 🛠️ Troubleshooting

**Lỗi Thường Gặp:**

1. **❌ "There is already an object named 'Brand' in the database"**
   ```bash
   # Giải pháp: Chạy setup script
   .\setup.ps1
   # Hoặc thủ công:
   dotnet ef database drop --force
   dotnet ef database update
   ```

2. **❌ "Failed to fetch" khi gọi API**
   - Sử dụng HTTP: `http://localhost:5228` thay vì HTTPS
   - Kiểm tra ứng dụng đã chạy: `dotnet run`
   - Test trên Swagger UI: http://localhost:5228/swagger

3. **❌ "Cannot connect to database"**
   - Kiểm tra SQL Server đang chạy
   - Cập nhật connection string trong `appsettings.json`
   - Sử dụng Windows Authentication

4. **❌ "Login failed for user"**
   - Sử dụng Windows Authentication trong connection string
   - Hoặc tạo SQL Server user với quyền phù hợp

5. **❌ "Port already in use"**
   - Ứng dụng sử dụng port 5228 (HTTP) và 7181 (HTTPS)
   - Kiểm tra không có ứng dụng nào khác sử dụng port này
   - Thay đổi port trong `launchSettings.json` nếu cần

6. **❌ "PowerShell execution policy"**
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

**💡 Mẹo:**
- Luôn sử dụng `.\setup.ps1` để tránh lỗi database
- Test API trên Swagger UI thay vì Postman/curl
- Sử dụng HTTP thay vì HTTPS để tránh lỗi certificate

## Testing with Swagger

1. Open Swagger UI
2. Use the "Authorize" button to add JWT token
3. Test the endpoints:
   - Register a new user
   - Login with credentials
   - Use the returned token for authenticated endpoints

## Default Roles

The system comes with pre-configured roles:
- **Admin** (ID: 1) - System Administrator
- **Station Staff** (ID: 2) - Station Staff Member  
- **EV Renter** (ID: 3) - Electric Vehicle Renter (default for new registrations)

## Security Features

- Password hashing using BCrypt
- JWT token authentication
- Role-based authorization
- Input validation
- CORS enabled for development

## Next Steps

This codebase provides the foundation for the EV Rental System. Future features can be added following the same architectural patterns:

- Vehicle management
- Station management
- Booking system
- Payment processing
- Reporting and analytics


