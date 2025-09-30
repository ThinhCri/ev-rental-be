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

## Getting Started

### Prerequisites

Before running this project, make sure you have the following installed:

- **.NET 8 SDK** - Download from [Microsoft's official site](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** - Either:
  - SQL Server Express (free) - Download from [Microsoft's official site](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
  - SQL Server LocalDB (included with Visual Studio)
  - SQL Server Developer Edition (free for development)

### Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone <your-github-repo-url>
   cd EV_RENTAL_SYSTEM
   ```

2. **Install Dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure Database Connection**
   
   Update the connection string in `appsettings.json` or `appsettings.Development.json`:
   
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EV_Rental_System;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```
   
   **For SQL Server Express:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EV_Rental_System;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

4. **Build the Project**
   ```bash
   dotnet build
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

6. **Access Swagger UI**
   Navigate to `https://localhost:7181/swagger` (or the port shown in the console)

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

### Troubleshooting

**Common Issues:**

1. **"Cannot connect to database"**
   - Check if SQL Server is running
   - Verify connection string
   - Make sure SQL Server allows TCP/IP connections

2. **"Login failed for user"**
   - Use Windows Authentication in connection string
   - Or create a SQL Server user with appropriate permissions

3. **"Database already exists" errors**
   - The application handles this automatically
   - If you see migration errors, the database might be in an inconsistent state
   - Consider dropping and recreating the database

4. **Port already in use**
   - The application will show which ports it's using
   - Make sure no other application is using those ports
   - You can change ports in `launchSettings.json`

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


