using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetByIdAsync(int id);
        Task<UserListResponseDto> GetAllAsync();
        Task<UserListResponseDto> SearchUsersAsync(UserSearchDto searchDto);
        Task<UserResponseDto> CreateAsync(CreateUserDto createDto);
        Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto updateDto);
        Task<UserResponseDto> DeleteAsync(int id);
        Task<UserResponseDto> ChangePasswordAsync(int id, ChangeUserPasswordDto changePasswordDto);
        Task<UserResponseDto> UpdateStatusAsync(int id, string status);
    }
}
