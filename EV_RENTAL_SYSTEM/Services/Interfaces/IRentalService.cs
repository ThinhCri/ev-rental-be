using EV_RENTAL_SYSTEM.Models.DTOs;

namespace EV_RENTAL_SYSTEM.Services.Interfaces
{
    public interface IRentalService
    {
        Task<RentalListResponseDto> GetAllRentalsAsync();
        Task<RentalResponseDto> GetRentalByIdAsync(int orderId);
        Task<RentalListResponseDto> GetUserRentalsAsync(int userId);
        Task<RentalResponseDto> CreateRentalAsync(CreateRentalDto createDto, int userId);
        Task<RentalResponseDto> CreateRentalWithMandatoryPaymentAsync(CreateRentalDto createDto, int userId);
        Task<RentalResponseDto> UpdateRentalAsync(int orderId, UpdateRentalDto updateDto);
        Task<RentalResponseDto> CancelRentalAsync(int orderId, int userId);
        Task<RentalResponseDto> CompleteRentalAsync(int orderId);
        Task<AvailableVehicleListResponseDto> GetAvailableVehiclesAsync(AvailableVehiclesSearchDto searchDto);
        Task<RentalListResponseDto> SearchRentalsAsync(RentalSearchDto searchDto);
        Task<RentalResponseDto> GetRentalWithDetailsAsync(int orderId);
        Task<decimal> CalculateRentalCostAsync(int vehicleId, DateTime startTime, DateTime endTime);
        Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime startTime, DateTime endTime);
        Task<ServiceResponse<ContractSummaryDto>> GetContractSummaryAsync(int orderId);
        Task<ServiceResponse<PaymentQrResponseDto>> ConfirmContractAsync(int orderId);
        Task<ServiceResponse<RentalResponseDto>> StaffConfirmRentalAsync(int orderId, StaffConfirmationDto request);
        Task<RentalListResponseDto> GetPendingOrdersAsync();
        Task<RentalResponseDto> StaffCancelOrderAsync(int orderId, string reason);
    }
}

