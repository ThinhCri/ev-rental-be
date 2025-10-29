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

                // Kiểm tra xem user có phải là nhân viên không (RoleId = 2 - Station Staff)
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
                    Message = "Đã xảy ra lỗi khi lấy thông tin nhân viên."
                };
            }
        }

        public async Task<StaffListResponseDto> GetAllAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var staffUsers = users.Where(u => u.RoleId == 2).ToList(); // Chỉ lấy Station Staff
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
                    Message = "Đã xảy ra lỗi khi lấy danh sách nhân viên."
                };
            }
        }

        public async Task<StaffListResponseDto> SearchStaffAsync(StaffSearchDto searchDto)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var query = users.Where(u => u.RoleId == 2).AsQueryable(); // Chỉ lấy Station Staff

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

                if (searchDto.StationId.HasValue)
                {
                    // Lấy danh sách user có StationId tương ứng
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
                    Message = "Đã xảy ra lỗi khi tìm kiếm nhân viên."
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
                    Message = "Đã xảy ra lỗi khi lấy danh sách nhân viên theo trạm."
                };
            }
        }

        public async Task<StaffResponseDto> CreateAsync(CreateStaffDto createDto)
        {
            try
            {
                // Kiểm tra trạm có tồn tại không
                var station = await _stationRepository.GetByIdAsync(createDto.StationId);
                if (station == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Station not found"
                    };
                }

                // Kiểm tra email đã tồn tại chưa
                if (await _userRepository.EmailExistsAsync(createDto.Email))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Email đã tồn tại trong hệ thống"
                    };
                }

                // Kiểm tra số điện thoại đã tồn tại chưa
                if (await _userRepository.PhoneNumberExistsAsync(createDto.PhoneNumber))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Số điện thoại đã tồn tại trong hệ thống"
                    };
                }

                // Tạo user entity với role Station Staff (RoleId = 2)
                var user = new User
                {
                    FullName = createDto.FullName,
                    Email = createDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                    PhoneNumber = createDto.PhoneNumber,
                    Birthday = createDto.Birthday,
                    CreatedAt = DateTime.UtcNow,
                    Status = createDto.Status ?? "Active",
                    RoleId = 2, // Station Staff
                    StationId = createDto.StationId, // FIX: Lưu StationId
                    Notes = createDto.Notes // FIX: Lưu Notes
                };

                // Lưu vào database
                var createdUser = await _userRepository.AddAsync(user);

                // FIX: Reload user with navigation properties
                var userWithNavProps = await _userRepository.GetByIdAsync(createdUser.UserId);
                if (userWithNavProps == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Lỗi khi tải thông tin nhân viên sau khi tạo"
                    };
                }

                // Map sang DTO để trả về
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
                    Message = "Đã xảy ra lỗi khi tạo nhân viên mới."
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

                // Kiểm tra nhân viên có tồn tại không
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy nhân viên với ID này."
                    };
                }

                // Kiểm tra xem user có phải là nhân viên không
                if (existingUser.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                // Kiểm tra trạm có tồn tại không
                var station = await _stationRepository.GetByIdAsync(updateDto.StationId);
                if (station == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Station not found"
                    };
                }

                // Kiểm tra email đã tồn tại chưa (trừ nhân viên hiện tại)
                if (updateDto.Email != existingUser.Email && await _userRepository.EmailExistsAsync(updateDto.Email))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Email đã tồn tại trong hệ thống"
                    };
                }

                // Kiểm tra số điện thoại đã tồn tại chưa (trừ nhân viên hiện tại)
                if (updateDto.PhoneNumber != existingUser.PhoneNumber && await _userRepository.PhoneNumberExistsAsync(updateDto.PhoneNumber))
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Số điện thoại đã tồn tại trong hệ thống"
                    };
                }

                // Cập nhật thông tin nhân viên
                existingUser.FullName = updateDto.FullName;
                existingUser.Email = updateDto.Email;
                existingUser.PhoneNumber = updateDto.PhoneNumber;
                existingUser.Birthday = updateDto.Birthday;
                existingUser.StationId = updateDto.StationId; // FIX: Cập nhật StationId
                existingUser.Notes = updateDto.Notes; // FIX: Cập nhật Notes
                if (!string.IsNullOrEmpty(updateDto.Status))
                {
                    existingUser.Status = updateDto.Status;
                }

                // Lưu vào database
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                // FIX: Reload user with navigation properties
                var userWithNavProps = await _userRepository.GetByIdAsync(updatedUser.UserId);
                if (userWithNavProps == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Lỗi khi tải thông tin nhân viên sau khi cập nhật"
                    };
                }

                // Map sang DTO để trả về
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
                    Message = "Đã xảy ra lỗi khi cập nhật nhân viên."
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

                // Kiểm tra nhân viên có tồn tại không
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy nhân viên với ID này."
                    };
                }

                // Kiểm tra xem user có phải là nhân viên không
                if (existingUser.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                // Xóa nhân viên
                var result = await _userRepository.DeleteAsync(id);
                if (!result)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Không thể xóa nhân viên."
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
                    Message = "Đã xảy ra lỗi khi xóa nhân viên."
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

                // Kiểm tra nhân viên có tồn tại không
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy nhân viên với ID này."
                    };
                }

                // Kiểm tra xem user có phải là nhân viên không
                if (existingUser.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                // Cập nhật mật khẩu
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

                // Lưu vào database
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                // FIX: Reload user with navigation properties
                var userWithNavProps = await _userRepository.GetByIdAsync(updatedUser.UserId);
                if (userWithNavProps == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Lỗi khi tải thông tin nhân viên sau khi đổi mật khẩu"
                    };
                }

                // Map sang DTO để trả về
                var staffDto = await MapToStaffDto(userWithNavProps);

                _logger.LogInformation("Staff password changed successfully with ID: {StaffId}", updatedUser.UserId);

                return new StaffResponseDto
                {
                    Success = true,
                    Message = "Mật khẩu đã được thay đổi thành công",
                    Data = staffDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password for staff with ID: {StaffId}", id);
                return new StaffResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi thay đổi mật khẩu."
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

                // Kiểm tra nhân viên có tồn tại không
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy nhân viên với ID này."
                    };
                }

                // Kiểm tra xem user có phải là nhân viên không
                if (existingUser.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                // Cập nhật trạng thái
                existingUser.Status = status;

                // Lưu vào database
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                // FIX: Reload user with navigation properties
                var userWithNavProps = await _userRepository.GetByIdAsync(updatedUser.UserId);
                if (userWithNavProps == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Lỗi khi tải thông tin nhân viên sau khi cập nhật trạng thái"
                    };
                }

                // Map sang DTO để trả về
                var staffDto = await MapToStaffDto(userWithNavProps);

                _logger.LogInformation("Staff status updated successfully with ID: {StaffId} to {Status}", updatedUser.UserId, status);

                return new StaffResponseDto
                {
                    Success = true,
                    Message = $"Trạng thái nhân viên đã được cập nhật thành {status}",
                    Data = staffDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for staff with ID: {StaffId}", id);
                return new StaffResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi cập nhật trạng thái nhân viên."
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

                // Kiểm tra nhân viên có tồn tại không
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy nhân viên với ID này."
                    };
                }

                // Kiểm tra xem user có phải là nhân viên không
                if (existingUser.RoleId != 2)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "User is not a staff member"
                    };
                }

                // Kiểm tra trạm mới có tồn tại không
                var newStation = await _stationRepository.GetByIdAsync(newStationId);
                if (newStation == null)
                {
                    return new StaffResponseDto
                    {
                        Success = false,
                        Message = "Trạm mới không tồn tại"
                    };
                }
                existingUser.StationId = newStationId;
                var updatedUser = await _userRepository.UpdateAsync(existingUser);

                var staffDto = await MapToStaffDto(updatedUser);

                _logger.LogInformation("Staff transferred successfully from station to station {StationId} for staff ID: {StaffId}", newStationId, id);

                return new StaffResponseDto
                {
                    Success = true,
                    Message = $"Staff tranfer to {newStation.StationName} Successfully",
                    Data = staffDto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while transferring staff with ID: {StaffId} to station: {StationId}", id, newStationId);
                return new StaffResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi khi chuyển nhân viên sang trạm khác."
                };
            }
        }

        // Helper methods
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