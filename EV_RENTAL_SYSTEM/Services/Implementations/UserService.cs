using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IMapper mapper,
            ILogger<UserService> logger) : base(logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> GetByIdAsync(int id)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    };
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var userDto = _mapper.Map<UserDto>(user);
                userDto.RoleName = user.Role.RoleName;

                return new UserResponseDto
                {
                    Success = true,
                    Message = "User information retrieved successfully",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy thông tin user."
                };
            }
        }

        public async Task<UserListResponseDto> GetAllAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.RoleName = user.Role.RoleName;
                    userDtos.Add(userDto);
                }

                return new UserListResponseDto
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = userDtos,
                    TotalCount = userDtos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                return new UserListResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi lấy danh sách user."
                };
            }
        }

        public async Task<UserListResponseDto> SearchUsersAsync(UserSearchDto searchDto)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var query = users.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchDto.FullName))
                {
                    query = query.Where(u => u.FullName.Contains(searchDto.FullName, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(searchDto.Email))
                {
                    query = query.Where(u => u.Email.Contains(searchDto.Email, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(searchDto.PhoneNumber))
                {
                    query = query.Where(u => u.PhoneNumber.Contains(searchDto.PhoneNumber));
                }

                if (searchDto.RoleId.HasValue)
                {
                    query = query.Where(u => u.RoleId == searchDto.RoleId.Value);
                }

                if (!string.IsNullOrEmpty(searchDto.Status))
                {
                    query = query.Where(u => u.Status == searchDto.Status);
                }

                if (searchDto.CreatedFrom.HasValue)
                {
                    query = query.Where(u => u.CreatedAt >= searchDto.CreatedFrom.Value);
                }

                if (searchDto.CreatedTo.HasValue)
                {
                    query = query.Where(u => u.CreatedAt <= searchDto.CreatedTo.Value);
                }

                // Apply sorting
                query = searchDto.SortBy?.ToLower() switch
                {
                    "fullname" => searchDto.SortOrder?.ToLower() == "asc" 
                        ? query.OrderBy(u => u.FullName) 
                        : query.OrderByDescending(u => u.FullName),
                    "email" => searchDto.SortOrder?.ToLower() == "asc" 
                        ? query.OrderBy(u => u.Email) 
                        : query.OrderByDescending(u => u.Email),
                    _ => searchDto.SortOrder?.ToLower() == "asc" 
                        ? query.OrderBy(u => u.CreatedAt) 
                        : query.OrderByDescending(u => u.CreatedAt)
                };

                var totalCount = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

                // Apply pagination
                var pagedUsers = query
                    .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                    .Take(searchDto.PageSize)
                    .ToList();

                var userDtos = new List<UserDto>();
                foreach (var user in pagedUsers)
                {
                    var userDto = _mapper.Map<UserDto>(user);
                    userDto.RoleName = user.Role.RoleName;
                    userDtos.Add(userDto);
                }

                return new UserListResponseDto
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = userDtos,
                    TotalCount = totalCount,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching users");
                return new UserListResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi tìm kiếm user."
                };
            }
        }

        public async Task<UserResponseDto> CreateAsync(CreateUserDto createDto)
        {
            try
            {
                // Kiểm tra role có tồn tại không
                var role = await _roleRepository.GetByIdAsync(createDto.RoleId);
                if (role == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                // Kiểm tra email đã tồn tại chưa
                if (await _userRepository.EmailExistsAsync(createDto.Email))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Email already exists."
                    };
                }

                // Kiểm tra số điện thoại đã tồn tại chưa
                if (await _userRepository.PhoneNumberExistsAsync(createDto.PhoneNumber))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Phone number already in use by another account. "
                    };
                }

                // Tạo user entity
                var user = _mapper.Map<User>(createDto);
                user.Password = BCrypt.Net.BCrypt.HashPassword(createDto.Password);
                user.CreatedAt = DateTime.UtcNow;
                user.Role = role;

                // Lưu vào database
                var createdUser = await _userRepository.AddAsync(user);

                // Map sang DTO để trả về
                var userDto = _mapper.Map<UserDto>(createdUser);
                userDto.RoleName = role.RoleName;

                _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.UserId);

                return new UserResponseDto
                {
                    Success = true,
                    Message = "User created successfully",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi tạo user mới."
                };
            }
        }

        public async Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto updateDto)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    };
                }

                // Kiểm tra user có tồn tại không
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy user với ID này."
                    };
                }

                // Kiểm tra role có tồn tại không
                var role = await _roleRepository.GetByIdAsync(updateDto.RoleId);
                if (role == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                // Kiểm tra email đã tồn tại chưa (trừ user hiện tại)
                if (updateDto.Email != existingUser.Email && await _userRepository.EmailExistsAsync(updateDto.Email))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Email đã tồn tại trong hệ thống"
                    };
                }

                // Kiểm tra số điện thoại đã tồn tại chưa (trừ user hiện tại)
                if (updateDto.PhoneNumber != existingUser.PhoneNumber && await _userRepository.PhoneNumberExistsAsync(updateDto.PhoneNumber))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Số điện thoại đã tồn tại trong hệ thống"
                    };
                }

                // Cập nhật thông tin user
                existingUser.FullName = updateDto.FullName;
                existingUser.Email = updateDto.Email;
                existingUser.PhoneNumber = updateDto.PhoneNumber;
                existingUser.Birthday = updateDto.Birthday;
                existingUser.RoleId = updateDto.RoleId;
                existingUser.Role = role;
                if (!string.IsNullOrEmpty(updateDto.Status))
                {
                    existingUser.Status = updateDto.Status;
                }

                // Lưu vào database
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                // Map sang DTO để trả về
                var userDto = _mapper.Map<UserDto>(updatedUser);
                userDto.RoleName = role.RoleName;

                _logger.LogInformation("User updated successfully with ID: {UserId}", updatedUser.UserId);

                return new UserResponseDto
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID: {UserId}", id);
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật user."
                };
            }
        }

        public async Task<UserResponseDto> DeleteAsync(int id)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    };
                }

                // Kiểm tra user có tồn tại không
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy user với ID này."
                    };
                }

                // Xóa user
                var result = await _userRepository.DeleteAsync(id);
                if (!result)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Không thể xóa user."
                    };
                }

                _logger.LogInformation("User deleted successfully with ID: {UserId}", id);

                return new UserResponseDto
                {
                    Success = true,
                    Message = "User deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi xóa user."
                };
            }
        }

        public async Task<UserResponseDto> ChangePasswordAsync(int id, ChangeUserPasswordDto changePasswordDto)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    };
                }

                // Kiểm tra user có tồn tại không
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy user với ID này."
                    };
                }

                // Cập nhật mật khẩu
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

                // Lưu vào database
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                // Map sang DTO để trả về
                var userDto = _mapper.Map<UserDto>(updatedUser);
                userDto.RoleName = updatedUser.Role.RoleName;

                _logger.LogInformation("User password changed successfully with ID: {UserId}", updatedUser.UserId);

                return new UserResponseDto
                {
                    Success = true,
                    Message = "Mật khẩu đã được thay đổi thành công",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password for user with ID: {UserId}", id);
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi thay đổi mật khẩu."
                };
            }
        }

        public async Task<UserResponseDto> UpdateStatusAsync(int id, string status)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    };
                }

                // Kiểm tra user có tồn tại không
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy user với ID này."
                    };
                }

                // Cập nhật trạng thái
                existingUser.Status = status;

                // Lưu vào database
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                // Map sang DTO để trả về
                var userDto = _mapper.Map<UserDto>(updatedUser);
                userDto.RoleName = updatedUser.Role.RoleName;

                _logger.LogInformation("User status updated successfully with ID: {UserId} to {Status}", updatedUser.UserId, status);

                return new UserResponseDto
                {
                    Success = true,
                    Message = $"Trạng thái user đã được cập nhật thành {status}",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for user with ID: {UserId}", id);
                return new UserResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật trạng thái user."
                };
            }
        }
    }
}
