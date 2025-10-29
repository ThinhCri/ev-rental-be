using AutoMapper;
using EV_RENTAL_SYSTEM.Models;
using EV_RENTAL_SYSTEM.Models.DTOs;
using EV_RENTAL_SYSTEM.Repositories.Interfaces;
using EV_RENTAL_SYSTEM.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace EV_RENTAL_SYSTEM.Services.Implementations
{
    public class StaffService : BaseService, IStaffService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IStationRepository _stationRepository;
        private readonly IMapper _mapper;

        public StaffService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IStationRepository stationRepository,
            IMapper mapper,
            ILogger<StaffService> logger) : base(logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _stationRepository = stationRepository;
            _mapper = mapper;
        }

        public async Task<StaffResponseDto> GetByIdAsync(int id)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Invalid staff ID"
                    };
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Staff not found"
                    };
                }

                if (user.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                var staffDto = await MapToStaffDto(user);

                return new StaffResponseDto
                {
                    Success = true,
                    Message = "Staff information retrieved successfully",
                    Data = staffDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff with ID: {StaffId}", id);
                return new StaffResponseDto
                {
                    Success = false,
                    Message = "An error occurred while retrieving staff information."
                };
            }
        }

        public async Task<StaffListResponseDto> GetAllAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var staffUsers = users.Where(u => u.RoleId == 2).ToList();
                var staffDtos = new List<StaffDto>();

                foreach (var user in staffUsers)
                {
                    var staffDto = await MapToStaffDto(user);
                    staffDtos.Add(staffDto);
                }

                return new StaffListResponseDto
                {
                    Success = true,
                    Message = "Staff retrieved successfully",
                    Data = staffDtos,
                    TotalCount = staffDtos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all staff");
                return new StaffListResponseDto
                {
                    Success = false,
                    Message = "An error occurred while retrieving staff list."
                };
            }
        }

        public async Task<StaffListResponseDto> SearchStaffAsync(StaffSearchDto searchDto)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var query = users.Where(u => u.RoleId == 2).AsQueryable();

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

                if (searchDto.StationId.HasValue)
                {
                    var stationUsers = await GetUsersByStationId(searchDto.StationId.Value);
                    var stationUserIds = stationUsers.Select(u => u.UserId).ToList();
                    query = query.Where(u => stationUserIds.Contains(u.UserId));
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

                var staffDtos = new List<StaffDto>();
                foreach (var user in pagedUsers)
                {
                    var staffDto = await MapToStaffDto(user);
                    staffDtos.Add(staffDto);
                }

                return new StaffListResponseDto
                {
                    Success = true,
                    Message = "Staff retrieved successfully",
                    Data = staffDtos,
                    TotalCount = totalCount,
                    PageNumber = searchDto.PageNumber,
                    PageSize = searchDto.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching staff");
                return new StaffListResponseDto
                {
                    Success = false,
                    Message = "An error occurred while searching staff."
                };
            }
        }

        public async Task<StaffListResponseDto> GetByStationIdAsync(int stationId)
        {
            try
            {
                if (!ValidateId(stationId, nameof(stationId)))
                {
                    return new StaffListResponseDto
                    {
                        Success = false,
                        Message = "Invalid station ID"
                    };
                }

                var stationUsers = await GetUsersByStationId(stationId);
                var staffDtos = new List<StaffDto>();

                foreach (var user in stationUsers)
                {
                    var staffDto = await MapToStaffDto(user);
                    staffDtos.Add(staffDto);
                }

                return new StaffListResponseDto
                {
                    Success = true,
                    Message = "Staff retrieved successfully",
                    Data = staffDtos,
                    TotalCount = staffDtos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting staff by station ID: {StationId}", stationId);
                return new StaffListResponseDto
                {
                    Success = false,
                    Message = "An error occurred while retrieving staff by station."
                };
            }
        }

        public async Task<StaffResponseDto> CreateAsync(CreateStaffDto createDto)
        {
            try
            {
                var station = await _stationRepository.GetByIdAsync(createDto.StationId);
                if (station == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Station not found"
                    };
                }

                if (await _userRepository.EmailExistsAsync(createDto.Email))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Email already exists in the system"
                    };
                }

                if (await _userRepository.PhoneNumberExistsAsync(createDto.PhoneNumber))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Phone number already exists in the system"
                    };
                }

                var user = new User
                {
                    FullName = createDto.FullName,
                    Email = createDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                    PhoneNumber = createDto.PhoneNumber,
                    Birthday = createDto.Birthday,
                    CreatedAt = DateTime.UtcNow,
                    Status = createDto.Status ?? "Active",
                    RoleId = 2,
                    StationId = createDto.StationId,
                    Notes = createDto.Notes
                };

                var createdUser = await _userRepository.AddAsync(user);

                var userWithNavProps = await _userRepository.GetByIdAsync(createdUser.UserId);
                if (userWithNavProps == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Error loading staff information after creation"
                    };
                }

                var staffDto = await MapToStaffDto(userWithNavProps);

                _logger.LogInformation("Staff created successfully with ID: {StaffId}", createdUser.UserId);

                return new StaffResponseDto
                {
                    Success = true,
                    Message = "Staff created successfully",
                    Data = staffDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating staff");
                return new StaffResponseDto
                {
                    Success = false,
                    Message = "An error occurred while creating new staff."
                };
            }
        }

        public async Task<StaffResponseDto> UpdateAsync(int id, UpdateStaffDto updateDto)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Invalid staff ID"
                    };
                }

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Staff not found with this ID."
                    };
                }

                if (existingUser.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                var station = await _stationRepository.GetByIdAsync(updateDto.StationId);
                if (station == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Station not found"
                    };
                }

                if (updateDto.Email != existingUser.Email && await _userRepository.EmailExistsAsync(updateDto.Email))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Email already exists in the system"
                    };
                }

                if (updateDto.PhoneNumber != existingUser.PhoneNumber && await _userRepository.PhoneNumberExistsAsync(updateDto.PhoneNumber))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Phone number already exists in the system"
                    };
                }

                existingUser.FullName = updateDto.FullName;
                existingUser.Email = updateDto.Email;
                existingUser.PhoneNumber = updateDto.PhoneNumber;
                existingUser.Birthday = updateDto.Birthday;
                existingUser.StationId = updateDto.StationId;
                existingUser.Notes = updateDto.Notes;
                if (!string.IsNullOrEmpty(updateDto.Status))
                {
                    existingUser.Status = updateDto.Status;
                }

                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                var userWithNavProps = await _userRepository.GetByIdAsync(updatedUser.UserId);
                if (userWithNavProps == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Error loading staff information after update"
                    };
                }

                var staffDto = await MapToStaffDto(userWithNavProps);

                _logger.LogInformation("Staff updated successfully with ID: {StaffId}", updatedUser.UserId);

                return new StaffResponseDto
                {
                    Success = true,
                    Message = "Staff updated successfully",
                    Data = staffDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating staff with ID: {StaffId}", id);
                return new StaffResponseDto
                {
                    Success = false,
                    Message = "An error occurred while updating staff."
                };
            }
        }

        public async Task<StaffResponseDto> DeleteAsync(int id)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Invalid staff ID"
                    };
                }

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Staff not found with this ID."
                    };
                }

                if (existingUser.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                var result = await _userRepository.DeleteAsync(id);
                if (!result)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Unable to delete staff."
                    };
                }

                _logger.LogInformation("Staff deleted successfully with ID: {StaffId}", id);

                return new StaffResponseDto
                {
                    Success = true,
                    Message = "Staff deleted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting staff with ID: {StaffId}", id);
                return new StaffResponseDto
                {
                    Success = false,
                    Message = "An error occurred while deleting staff."
                };
            }
        }

        public async Task<StaffResponseDto> ChangePasswordAsync(int id, ChangeUserPasswordDto changePasswordDto)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Invalid staff ID"
                    };
                }

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Staff not found with this ID."
                    };
                }

                if (existingUser.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                var userWithNavProps = await _userRepository.GetByIdAsync(updatedUser.UserId);
                if (userWithNavProps == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Error loading staff information after changing password"
                    };
                }

                var staffDto = await MapToStaffDto(userWithNavProps);

                _logger.LogInformation("Staff password changed successfully with ID: {StaffId}", updatedUser.UserId);

                return new StaffResponseDto
                {
                    Success = true,
                    Message = "Password changed successfully",
                    Data = staffDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password for staff with ID: {StaffId}", id);
                return new StaffResponseDto
                {
                    Success = false,
                    Message = "An error occurred while changing password."
                };
            }
        }

        public async Task<StaffResponseDto> UpdateStatusAsync(int id, string status)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Invalid staff ID"
                    };
                }

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Staff not found with this ID."
                    };
                }

                if (existingUser.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                existingUser.Status = status;

                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                var userWithNavProps = await _userRepository.GetByIdAsync(updatedUser.UserId);
                if (userWithNavProps == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Error loading staff information after updating status"
                    };
                }

                var staffDto = await MapToStaffDto(userWithNavProps);

                _logger.LogInformation("Staff status updated successfully with ID: {StaffId} to {Status}", updatedUser.UserId, status);

                return new StaffResponseDto
                {
                    Success = true,
                    Message = $"Staff status updated to {status}",
                    Data = staffDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for staff with ID: {StaffId}", id);
                return new StaffResponseDto
                {
                    Success = false,
                    Message = "An error occurred while updating staff status."
                };
            }
        }

        public async Task<StaffResponseDto> TransferStationAsync(int id, int newStationId)
        {
            try
            {
                if (!ValidateId(id, nameof(id)))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Invalid staff ID"
                    };
                }

                if (!ValidateId(newStationId, nameof(newStationId)))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Invalid station ID"
                    };
                }

                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Staff not found with this ID."
                    };
                }

                if (existingUser.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                var newStation = await _stationRepository.GetByIdAsync(newStationId);
                if (newStation == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "New station does not exist"
                    };
                }
                existingUser.StationId = newStationId;
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                var staffDto = await MapToStaffDto(updatedUser);

                _logger.LogInformation("Staff transferred successfully from station to station {StationId} for staff ID: {StaffId}", newStationId, id);

                return new StaffResponseDto
                {
                    Success = true,
                    Message = $"Staff transferred to {newStation.StationName} successfully",
                    Data = staffDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while transferring staff with ID: {StaffId} to station: {StationId}", id, newStationId);
                return new StaffResponseDto
                {
                    Success = false,
                    Message = "An error occurred while transferring staff to another station."
                };
            }
        }

        private async Task<StaffDto> MapToStaffDto(User user, int? stationId = null, string? notes = null)
        {
            var staffDto = new StaffDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday,
                CreatedAt = user.CreatedAt,
                Status = user.Status,
                RoleName = user.Role?.RoleName ?? "Unknown",
                Notes = notes ?? user.Notes,
                StationId = 0,
                StationName = ""
            };

            if (user.Station != null)
            {
                staffDto.StationId = user.Station.StationId;
                staffDto.StationName = user.Station.StationName;
            }
            else
            {
                var targetStationId = user.StationId ?? stationId;
                if (targetStationId.HasValue)
                {
                    var station = await _stationRepository.GetByIdAsync(targetStationId.Value);
                    if (station != null)
                    {
                        staffDto.StationId = station.StationId;
                        staffDto.StationName = station.StationName;
                    }
                }
                else
                {
                    staffDto.StationId = 0;
                    staffDto.StationName = "Not Staff";
                }
            }

            return staffDto;
        }

        private async Task<List<User>> GetUsersByStationId(int stationId)
        {
            var users = await _userRepository.GetAllAsync();
            return users.Where(u => u.RoleId == 2 && u.StationId == stationId).ToList();
        }
    }
}
