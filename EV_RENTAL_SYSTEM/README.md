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

1. **Install Dependencies**
   ```bash
   dotnet restore
   ```

2. **Update Database**
   The application will automatically create the database and tables on first run.

3. **Run the Application**
   ```bash
   dotnet run
   ```

4. **Access Swagger UI**
   Navigate to `https://localhost:7000/swagger` (or the port shown in the console)

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


