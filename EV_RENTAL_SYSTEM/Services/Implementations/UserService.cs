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
        private readonly ICloudService _cloudService;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IMapper mapper,
            ICloudService cloudService,
            IUnitOfWork unitOfWork,
            ILogger<UserService> logger) : base(logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _cloudService = cloudService;
            _unitOfWork = unitOfWork;
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
                    Message = "An error occurred while retrieving user information."
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
                    Message = "An error occurred while retrieving user list."
                };
            }
        }

        public async Task<UserListResponseDto> SearchUsersAsync(UserSearchDto searchDto)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var query = users.AsQueryable();

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
                    Message = "An error occurred while searching users."
                };
            }
        }

        public async Task<UserResponseDto> CreateAsync(CreateUserDto createDto)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(createDto.RoleId);
                if (role == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                if (await _userRepository.EmailExistsAsync(createDto.Email))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Email already exists in the system"
                    };
                }

                if (await _userRepository.PhoneNumberExistsAsync(createDto.PhoneNumber))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Phone number already exists in the system"
                    };
                }

                var user = _mapper.Map<User>(createDto);
                user.Password = BCrypt.Net.BCrypt.HashPassword(createDto.Password);
                user.CreatedAt = DateTime.UtcNow;
                user.Role = role;

                var createdUser = await _userRepository.AddAsync(user);

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
                    Message = "An error occurred while creating new user."
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

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "User with this ID not found."
                    };
                }

                var role = await _roleRepository.GetByIdAsync(updateDto.RoleId);
                if (role == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Role not found"
                    };
                }

                if (updateDto.Email != existingUser.Email && await _userRepository.EmailExistsAsync(updateDto.Email))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Email already exists in the system"
                    };
                }

                if (updateDto.PhoneNumber != existingUser.PhoneNumber && await _userRepository.PhoneNumberExistsAsync(updateDto.PhoneNumber))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Phone number already exists in the system"
                    };
                }

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

                var updatedUser = await _userRepository.UpdateAsync(existingUser);

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
                    Message = "An error occurred while updating user."
                };
            }
        }

        public async Task<UserResponseDto> UpdateProfileAsync(int userId, UpdateUserProfileDto updateDto)
        {
            try
            {
                if (!ValidateId(userId, nameof(userId)))
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    };
                }

                var existingUser = await _userRepository.GetByIdAsync(userId);
                if (existingUser == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "User with this ID not found."
                    };
                }

                if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != "string" && updateDto.Email != "user@example.com" && updateDto.Email.Trim() != "")
                {
                    var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                    if (!emailRegex.IsMatch(updateDto.Email))
                    {
                        return new UserResponseDto
                        {
                            Success = false,
                            Message = "Invalid email format"
                        };
                    }

                    if (updateDto.Email != existingUser.Email && await _userRepository.EmailExistsAsync(updateDto.Email))
                    {
                        return new UserResponseDto
                        {
                            Success = false,
                            Message = "Email already exists in the system"
                        };
                    }
                }

                if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber) && updateDto.PhoneNumber != existingUser.PhoneNumber)
                {
                    if (await _userRepository.PhoneNumberExistsAsync(updateDto.PhoneNumber))
                    {
                        return new UserResponseDto
                        {
                            Success = false,
                            Message = "Phone number already exists in the system"
                        };
                    }
                }

                if (!string.IsNullOrWhiteSpace(updateDto.NewPassword) && updateDto.NewPassword != "string" && updateDto.NewPassword.Trim() != "")
                {
                    if (updateDto.NewPassword.Length < 6)
                    {
                        return new UserResponseDto
                        {
                            Success = false,
                            Message = "New password must be at least 6 characters"
                        };
                    }

                    if (string.IsNullOrWhiteSpace(updateDto.CurrentPassword) || updateDto.CurrentPassword == "string")
                    {
                        return new UserResponseDto
                        {
                            Success = false,
                            Message = "Current password is required to change password"
                        };
                    }

                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(updateDto.CurrentPassword, existingUser.Password);
                    if (!isPasswordValid)
                    {
                        return new UserResponseDto
                        {
                            Success = false,
                            Message = "Current password is incorrect."
                        };
                    }

                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
                }

                var userLicenses = await _unitOfWork.Licenses.GetByUserIdAsync(userId);
                var existingLicense = userLicenses.FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(updateDto.LicenseNumber) && updateDto.LicenseNumber != "string" && updateDto.LicenseNumber.Trim() != "")
                {
                    if (existingLicense != null)
                    {
                        existingLicense.LicenseNumber = updateDto.LicenseNumber.Trim();
                        _unitOfWork.Licenses.Update(existingLicense);
                    }
                    else
                    {
                        _logger.LogWarning("User {UserId} tried to update license number but has no existing license", userId);
                    }
                }

                if (updateDto.LicenseImage != null)
                {
                    if (existingLicense == null)
                    {
                        return new UserResponseDto
                        {
                            Success = false,
                            Message = "User must have a license to upload license image"
                        };
                    }

                    string? oldLicenseImageUrl = existingLicense.LicenseImageUrl;

                    var newLicenseImageUrl = await _cloudService.UploadLicenseImageAsync(updateDto.LicenseImage);

                    if (!string.IsNullOrEmpty(oldLicenseImageUrl))
                    {
                        try
                        {
                            await _cloudService.DeleteImageAsync(oldLicenseImageUrl);
                            _logger.LogInformation("Deleted old license image for user {UserId}: {OldImageUrl}", userId, oldLicenseImageUrl);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete old license image for user {UserId}: {OldImageUrl}", userId, oldLicenseImageUrl);
                        }
                    }

                    existingLicense.LicenseImageUrl = newLicenseImageUrl;
                    _unitOfWork.Licenses.Update(existingLicense);
                }

                if (!string.IsNullOrWhiteSpace(updateDto.FullName) && updateDto.FullName != "string" && updateDto.FullName.Trim() != "")
                {
                    existingUser.FullName = updateDto.FullName.Trim();
                }

                if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != "string" && updateDto.Email != "user@example.com" && updateDto.Email.Trim() != "")
                {
                    existingUser.Email = updateDto.Email.Trim();
                }

                if (!string.IsNullOrWhiteSpace(updateDto.PhoneNumber) && updateDto.PhoneNumber != "string" && updateDto.PhoneNumber.Trim() != "")
                {
                    existingUser.PhoneNumber = updateDto.PhoneNumber.Trim();
                }

                if ((!string.IsNullOrWhiteSpace(updateDto.LicenseNumber) || updateDto.LicenseImage != null) && existingLicense != null)
                {
                    await _unitOfWork.SaveChangesAsync();
                }

                var updatedUser = await _userRepository.UpdateAsync(existingUser);


                var userWithNavProps = await _userRepository.GetByIdAsync(updatedUser.UserId);
                if (userWithNavProps == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Error loading user information after update"
                    };
                }

                var userDto = _mapper.Map<UserDto>(userWithNavProps);
                userDto.RoleName = userWithNavProps.Role.RoleName;

                var licenses = await _unitOfWork.Licenses.GetByUserIdAsync(userId);
                var license = licenses.FirstOrDefault();
                if (license != null)
                {
                    userDto.DriverLicenseNumber = license.LicenseNumber;
                    userDto.DriverLicenseImage = license.LicenseImageUrl;
                }

                _logger.LogInformation("User profile updated successfully with ID: {UserId}", updatedUser.UserId);

                return new UserResponseDto
                {
                    Success = true,
                    Message = "Profile updated successfully",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating profile for user with ID: {UserId}", userId);
                return new UserResponseDto
                {
                    Success = false,
                    Message = "An error occurred while updating profile."
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

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "User with this ID not found."
                    };
                }

                var result = await _userRepository.DeleteAsync(id);
                if (!result)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Unable to delete user."
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
                    Message = "An error occurred while deleting user."
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

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "User with this ID not found."
                    };
                }

                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
                var updatedUser = await _userRepository.UpdateAsync(existingUser);
                var userDto = _mapper.Map<UserDto>(updatedUser);
                userDto.RoleName = updatedUser.Role.RoleName;
                _logger.LogInformation("User password changed successfully with ID: {UserId}", updatedUser.UserId);
                return new UserResponseDto
                {
                    Success = true,
                    Message = "Password changed successfully",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password for user with ID: {UserId}", id);
                return new UserResponseDto
                {
                    Success = false,
                    Message = "An error occurred while changing password."
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

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "User with this ID not found."
                    };
                }

                existingUser.Status = status;
                var updatedUser = await _userRepository.UpdateAsync(existingUser);
                var userWithNavProps = await _userRepository.GetByIdAsync(updatedUser.UserId);
                if (userWithNavProps == null)
                {
                    return new UserResponseDto
                    {
                        Success = false,
                        Message = "Error loading user information after updating status"
                    };
                }
                var userDto = _mapper.Map<UserDto>(userWithNavProps);
                userDto.RoleName = userWithNavProps.Role.RoleName;
                _logger.LogInformation("User status updated successfully with ID: {UserId} to {Status}", updatedUser.UserId, status);
                return new UserResponseDto
                {
                    Success = true,
                    Message = $"User status updated to {status}",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for user with ID: {UserId}", id);
                return new UserResponseDto
                {
                    Success = false,
                    Message = "An error occurred while updating user status."
                };
            }
        }
    }
}
