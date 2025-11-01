using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly ICloudService _cloudService;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _memoryCache;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<AuthService> logger,
            ICloudService cloudService,
            IEmailService emailService,
            IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
            _cloudService = cloudService;
            _emailService = emailService;
            _memoryCache = memoryCache;
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

        public async Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(forgotPasswordDto.Email);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = true,
                        Message = "If email exists, OTP has been sent."
                    };
                }

                var otp = new Random().Next(100000, 999999).ToString();
                var cacheKey = $"reset_password_{forgotPasswordDto.Email.ToLower()}";
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(55)
                };

                _memoryCache.Set(cacheKey, otp, cacheOptions);

                var emailSubject = "Password Reset OTP - EV Rental System";
                var emailBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #2c3e50; text-align: center;'>Password Reset Request</h2>
                            <h3 style='color: #34495e;'>Hello {user.FullName},</h3>
                            <p>You have requested to reset your password. Please use the following OTP code:</p>
                            <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0; text-align: center;'>
                                <h1 style='color: #3498db; font-size: 32px; margin: 0; letter-spacing: 5px;'>{otp}</h1>
                            </div>
                            <p><strong>This OTP will expire in 5 minutes.</strong></p>
                            <p>If you did not request this password reset, please ignore this email.</p>
                            <p style='text-align: center; color: #7f8c8d; font-size: 12px; margin-top: 30px;'>
                                Best regards,<br>
                                <strong>EV Rental System Team</strong>
                            </p>
                        </div>
                    </body>
                    </html>";

                await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

                _logger.LogInformation("Password reset OTP sent to {Email}", user.Email);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "OTP has been sent to your email."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during forgot password for email: {Email}", forgotPasswordDto.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred. Please try again."
                };
            }
        }

        public async Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or OTP."
                    };
                }

                var cacheKey = $"reset_password_{resetPasswordDto.Email.ToLower()}";
                if (!_memoryCache.TryGetValue(cacheKey, out string? cachedOtp) || cachedOtp != resetPasswordDto.Otp)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired OTP."
                    };
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _memoryCache.Remove(cacheKey);

                _logger.LogInformation("Password reset successfully for {Email}", user.Email);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Password has been reset successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during reset password for email: {Email}", resetPasswordDto.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred. Please try again."
                };
            }
        }

    }
}
