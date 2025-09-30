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
        private readonly IFileService _fileService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<AuthService> logger,
            IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
            _fileService = fileService;
        }

        /// <summary>
        /// Xử lý đăng nhập người dùng
        /// </summary>
        /// <param name="loginRequest">Thông tin đăng nhập</param>
        /// <returns>Kết quả đăng nhập</returns>
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                // 1. Tìm user theo email trong database
                var user = await _unitOfWork.Users.GetByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                // 2. Kiểm tra password có đúng không (so sánh với hash đã lưu)
                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                // 3. Kiểm tra tài khoản có đang active không
                if (user.Status != "Active")
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Account is not active. Please contact support."
                    };
                }

                // 4. Tạo JWT token cho user
                var token = _jwtService.GenerateToken(user);
                
                // 5. Map user entity sang DTO để trả về
                var userDto = _mapper.Map<UserDto>(user);
                userDto.RoleName = user.Role.RoleName;

                // 6. Ghi log đăng nhập thành công
                _logger.LogInformation("User {Email} logged in successfully", user.Email);

                // 7. Trả về kết quả đăng nhập thành công
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
                // Ghi log lỗi và trả về thông báo lỗi
                _logger.LogError(ex, "Error occurred during login for email: {Email}", loginRequest.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during login. Please try again."
                };
            }
        }

        /// <summary>
        /// Xử lý đăng ký tài khoản người dùng mới
        /// </summary>
        /// <param name="registerRequest">Thông tin đăng ký</param>
        /// <returns>Kết quả đăng ký</returns>
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            // Bắt đầu transaction để đảm bảo tính toàn vẹn dữ liệu
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Kiểm tra email đã tồn tại trong database chưa
                if (await _unitOfWork.Users.EmailExistsAsync(registerRequest.Email))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email already exists."
                    };
                }

                // 2. Lấy role "EV Renter" từ database (role mặc định cho người đăng ký)
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

                // 3. VALIDATION TRƯỚC KHI LƯU USER - Kiểm tra tất cả thông tin license trước
            // Kiểm tra loại bằng lái xe có tồn tại trong database không
            var licenseTypeName = registerRequest.LicenseTypeId.ToString();
            var licenseType = await _unitOfWork.LicenseTypes.GetByNameAsync(licenseTypeName);
            if (licenseType == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Loại bằng lái xe không tồn tại."
                };
            }

                // Kiểm tra ảnh bằng lái xe có hợp lệ không
                var imageValidation = await _fileService.ValidateLicenseImageAsync(registerRequest.LicenseImage);
                if (!imageValidation.IsValid)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = $"Ảnh bằng lái xe không hợp lệ: {imageValidation.ErrorMessage}"
                    };
                }

                // Kiểm tra số bằng lái xe đã tồn tại chưa
                var existingLicense = await _unitOfWork.Licenses.GetByLicenseNumberAsync(registerRequest.LicenseNumber);
                if (existingLicense != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Số bằng lái xe đã được sử dụng bởi tài khoản khác."
                    };
                }

                // Kiểm tra ngày hết hạn bằng lái xe
                if (registerRequest.LicenseExpiryDate <= DateTime.Now)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Bằng lái xe đã hết hạn. Vui lòng cập nhật bằng lái xe mới."
                    };
                }

                // 4. Tạo user mới với thông tin từ request
                var user = new User
                {
                    FullName = registerRequest.FullName,
                    Email = registerRequest.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password), // Hash password trước khi lưu
                    Birthday = registerRequest.Birthday,
                    Status = "Active", // Mặc định active
                    RoleId = evRenterRole.RoleId, // Gán role EV Renter
                    CreatedAt = DateTime.UtcNow
                };

                // 5. Lưu user vào database
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // 6. Upload ảnh bằng lái xe
                var licenseImageUrl = await _fileService.UploadCccdImageAsync(registerRequest.LicenseImage, user.UserId);

                // 7. Tạo license record
                var license = new License
                {
                    LicenseNumber = registerRequest.LicenseNumber,
                    ExpiryDate = registerRequest.LicenseExpiryDate,
                    UserId = user.UserId,
                    LicenseTypeId = licenseTypeName, // Convert enum to string
                    LicenseImageUrl = licenseImageUrl
                };

                await _unitOfWork.Licenses.AddAsync(license);
                await _unitOfWork.SaveChangesAsync();

                // 8. Commit transaction - tất cả dữ liệu đã được lưu thành công
                await transaction.CommitAsync();

                // 9. Tạo JWT token để user có thể đăng nhập ngay
                var token = _jwtService.GenerateToken(user);
                
                // 10. Map user entity sang DTO để trả về
                var userDto = _mapper.Map<UserDto>(user);
                userDto.RoleName = evRenterRole.RoleName;

                // 11. Ghi log đăng ký thành công
                _logger.LogInformation("New user registered with email: {Email}", user.Email);

                // 12. Trả về kết quả đăng ký thành công
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
                // Rollback transaction nếu có lỗi xảy ra
                await _unitOfWork.RollbackTransactionAsync();
                
                // Ghi log lỗi và trả về thông báo lỗi
                _logger.LogError(ex, "Error occurred during registration for email: {Email}", registerRequest.Email);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "An error occurred during registration. Please try again."
                };
            }
            finally
            {
                // Dispose transaction
                transaction?.Dispose();
            }
        }

        /// <summary>
        /// Xử lý đăng xuất người dùng
        /// </summary>
        /// <param name="token">JWT token cần validate</param>
        /// <returns>Kết quả đăng xuất</returns>
        public Task<bool> LogoutAsync(string token)
        {
            try
            {
                // Trong ứng dụng thực tế, có thể cần blacklist token
                // Hiện tại chỉ validate token có hợp lệ không
                return Task.FromResult(_jwtService.ValidateToken(token));
            }
            catch (Exception ex)
            {
                // Ghi log lỗi và trả về false
                _logger.LogError(ex, "Error occurred during logout");
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Kiểm tra JWT token có hợp lệ không
        /// </summary>
        /// <param name="token">JWT token cần kiểm tra</param>
        /// <returns>Token có hợp lệ hay không</returns>
        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                // Gọi JWT service để validate token
                return Task.FromResult(_jwtService.ValidateToken(token));
            }
            catch (Exception ex)
            {
                // Ghi log lỗi và trả về false
                _logger.LogError(ex, "Error occurred during token validation");
                return Task.FromResult(false);
            }
        }

    }
}
