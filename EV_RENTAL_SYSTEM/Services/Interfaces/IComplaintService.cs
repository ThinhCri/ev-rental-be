using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IComplaintService
    {
        Task<ComplaintResponseDto> CreateComplaintAsync(CreateComplaintDto createDto, int userId);
        Task<ComplaintListResponseDto> GetAllComplaintsAsync();
        Task<ComplaintResponseDto> UpdateComplaintStatusAsync(int complaintId, UpdateComplaintStatusDto updateDto);
        Task<ComplaintListResponseDto> GetUserComplaintsAsync(int userId);
        Task<ComplaintListResponseDto> GetComplaintsByStationAsync(int stationId);
    }
}

