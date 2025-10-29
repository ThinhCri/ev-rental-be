using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly ICloudService _cloudService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<AuthService> logger,
            ICloudService cloudService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
            _cloudService = cloudService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                if (user.Status != "Active")
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Account is not active. Please contact support."
                    };
                }

                var token = _jwtService.GenerateToken(user);
                
                var userDto = _mapper.Map<UserDto>(user);
                userDto.RoleName = user.Role.RoleName;

                _logger.LogInformation("User {Email} logged in successfully", user.Email);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful.",
                    Token = token,
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email: {Email}", loginRequest.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during login. Please try again."
                };
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (await _unitOfWork.Users.EmailExistsAsync(registerRequest.Email))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email already exists."
                    };
                }

                if (await _unitOfWork.Users.PhoneNumberExistsAsync(registerRequest.PhoneNumber))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Phone number already in use by another account."
                    };
                }

                var evRenterRole = await _unitOfWork.Roles.GetByNameAsync("EV Renter");
                if (evRenterRole == null)
                {
                    _logger.LogError("EV Renter role not found in database");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "System configuration error. Please contact support."
                    };
                }

                var licenseTypeName = registerRequest.LicenseTypeId.ToString();
                var licenseType = await _unitOfWork.LicenseTypes.GetByNameAsync(licenseTypeName);
                if (licenseType == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "License is not valid"
                    };
                }

                // License image validation is now handled by CloudService

                var existingLicense = await _unitOfWork.Licenses.GetByLicenseNumberAsync(registerRequest.LicenseNumber);
                if (existingLicense != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Driver's license already exists"
                    };
                }

                if (registerRequest.LicenseExpiryDate <= DateTime.Now)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Driver's license has expired. Please update with a new license."
                    };
                }

                var user = new User
                {
                    FullName = registerRequest.FullName,
                    Email = registerRequest.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
                    PhoneNumber = registerRequest.PhoneNumber,
                    Birthday = registerRequest.Birthday,
                    Status = "Active",
                    RoleId = evRenterRole.RoleId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var licenseImageUrl = await _cloudService.UploadLicenseImageAsync(registerRequest.LicenseImage);

                var license = new License
                {
                    LicenseNumber = registerRequest.LicenseNumber,
                    ExpiryDate = registerRequest.LicenseExpiryDate,
                    UserId = user.UserId,
                    LicenseTypeId = licenseTypeName,
                    LicenseImageUrl = licenseImageUrl
                };

                await _unitOfWork.Licenses.AddAsync(license);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                var token = _jwtService.GenerateToken(user);
                
                var userDto = _mapper.Map<UserDto>(user);
                userDto.RoleName = evRenterRole.RoleName;

                _logger.LogInformation("New user registered with email: {Email}", user.Email);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Registration successful.",
                    Token = token,
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                
                _logger.LogError(ex, "Error occurred during registration for email: {Email}", registerRequest.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during registration. Please try again."
                };
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        public Task<bool> LogoutAsync(string token)
        {
            try
            {
                return Task.FromResult(_jwtService.ValidateToken(token));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout");
                return Task.FromResult(false);
            }
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                return Task.FromResult(_jwtService.ValidateToken(token));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token validation");
                return Task.FromResult(false);
            }
        }

    }
}
