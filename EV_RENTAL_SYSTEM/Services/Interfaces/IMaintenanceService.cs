using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<MaintenanceListResponseDto> GetAllMaintenancesAsync();
        Task<MaintenanceResponseDto> GetMaintenanceByIdAsync(int maintenanceId);
        Task<MaintenanceResponseDto> CreateMaintenanceAsync(CreateMaintenanceDto createDto);
        Task<MaintenanceResponseDto> UpdateMaintenanceAsync(int maintenanceId, UpdateMaintenanceDto updateDto);
        Task<MaintenanceResponseDto> DeleteMaintenanceAsync(int maintenanceId);
        Task<MaintenanceListResponseDto> SearchMaintenancesAsync(MaintenanceSearchDto searchDto);
        Task<MaintenanceListResponseDto> GetMaintenancesByLicensePlateIdAsync(int licensePlateId);
    }
}

