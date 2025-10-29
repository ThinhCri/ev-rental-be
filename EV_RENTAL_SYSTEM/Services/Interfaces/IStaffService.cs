using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IStaffService
    {
        Task<StaffResponseDto> GetByIdAsync(int id);
        Task<StaffListResponseDto> GetAllAsync();
        Task<StaffListResponseDto> SearchStaffAsync(StaffSearchDto searchDto);
        Task<StaffListResponseDto> GetByStationIdAsync(int stationId);
        Task<StaffResponseDto> CreateAsync(CreateStaffDto createDto);
        Task<StaffResponseDto> UpdateAsync(int id, UpdateStaffDto updateDto);
        Task<StaffResponseDto> DeleteAsync(int id);
        Task<StaffResponseDto> ChangePasswordAsync(int id, ChangeUserPasswordDto changePasswordDto);
        Task<StaffResponseDto> UpdateStatusAsync(int id, string status);
        Task<StaffResponseDto> TransferStationAsync(int id, int newStationId);
    }
}
