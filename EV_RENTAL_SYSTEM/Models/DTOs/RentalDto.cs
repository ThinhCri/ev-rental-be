using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    public class RentalDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Status { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? LicenseImageUrl { get; set; } 
        public int? ContractId { get; set; } 
        
        public int? LicenseId { get; set; }
        public string? LicenseNumber { get; set; }
        public string? LicenseType { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        
        public List<RentalVehicleDto> Vehicles { get; set; } = new List<RentalVehicleDto>();
        public List<RentalContractDto> Contracts { get; set; } = new List<RentalContractDto>();
        public int TotalDays { get; set; }
        public decimal? DailyRate { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? RentalFee { get; set; }
        public decimal? ExtraFee { get; set; }
    }

    public class RentalVehicleDto
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string? VehicleType { get; set; }
        public decimal? PricePerDay { get; set; }
        public int? SeatNumber { get; set; }
        public string? VehicleImage { get; set; }
        public string? StationName { get; set; }
        public List<RentalLicensePlateDto> LicensePlates { get; set; } = new List<RentalLicensePlateDto>();
    }

    public class RentalLicensePlateDto
    {
        public int LicensePlateId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string? Status { get; set; }
    }

    public class RentalContractDto
    {
        public int ContractId { get; set; }
        public string? ContractCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
        public decimal? Deposit { get; set; }
        public decimal? RentalFee { get; set; }
        public decimal? ExtraFee { get; set; }
        public string? HandoverImage { get; set; }
        public string? ReturnImage { get; set; }
        public List<RentalPaymentDto> Payments { get; set; } = new List<RentalPaymentDto>();
    }

    public class ContractSummaryDto
    {
        public string ContractCode { get; set; } = string.Empty;
        public decimal RentalFee { get; set; }
        public decimal Deposit { get; set; }
        public decimal OverKmFee { get; set; } = 0;
        public decimal ElectricityFee { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List<ContractFeeDetailDto> FeeDetails { get; set; } = new List<ContractFeeDetailDto>();
    }

    public class ContractFeeDetailDto
    {
        public string FeeType { get; set; } = string.Empty;
        public string FeeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class StaffConfirmationDto
    {
        [Required(ErrorMessage = "Confirmation status is required")]
        public bool IsConfirmed { get; set; }
        
        [MaxLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
        public string? Notes { get; set; }
        
        public string Action { get; set; } = string.Empty;
    }

    public class HandoverVehicleDto
    {
        [Required(ErrorMessage = "Vehicle image is required")]
        public IFormFile? VehicleImage { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
        public string? Notes { get; set; }
        
        [Required(ErrorMessage = "Odometer is required")]
        [Range(0, 999999, ErrorMessage = "Odometer must be between 0 and 999999 km")]
        public int Odometer { get; set; }
        
        [Required(ErrorMessage = "Battery level is required")]
        [Range(0, 100, ErrorMessage = "Battery must be between 0 and 100%")]
        public decimal Battery { get; set; }
    }

    public class ReturnVehicleDto
    {
        [Required(ErrorMessage = "Vehicle image is required")]
        public IFormFile? VehicleImage { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Notes must not exceed 1000 characters")]
        public string? Notes { get; set; }
        
        [Required(ErrorMessage = "Mileage is required")]
        [Range(0, 999999, ErrorMessage = "Mileage must be between 0 and 999999 km")]
        public int Odometer { get; set; }
        
        [Required(ErrorMessage = "Battery level is required")]
        [Range(0, 100, ErrorMessage = "Battery must be between 0 and 100%")]
        public decimal Battery { get; set; }
    }

    public class PaymentQrResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string QrCodeUrl { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public string ContractCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class RentalPaymentDto
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }

 
    public class CreateRentalDto
    {
        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }
        [Required(ErrorMessage = "Vehicle ID is required")]
        public int VehicleId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Deposit must be greater than or equal to 0")]
        public decimal? DepositAmount { get; set; }
        public bool IsBookingForOthers { get; set; } = false;
        public IFormFile? RenterLicenseImage { get; set; }
    }

    public class UpdateRentalDto
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<int>? VehicleIds { get; set; }
        public decimal? DepositAmount { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public IFormFile? RenterLicenseImage { get; set; }
    }

    public class AvailableVehiclesSearchDto
    {
        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }
        public int? StationId { get; set; }
        public int? BrandId { get; set; }
        public string? VehicleType { get; set; }
        public int? MinSeatNumber { get; set; }
        public int? MaxSeatNumber { get; set; }
        public decimal? MinPricePerDay { get; set; }
        public decimal? MaxPricePerDay { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AvailableVehicleDto
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string? VehicleType { get; set; }
        public decimal? PricePerDay { get; set; }
        public int? SeatNumber { get; set; }
        public string? VehicleImage { get; set; }
        public string? StationName { get; set; }
        public int? StationId { get; set; }
        public decimal? Battery { get; set; }
        public int? RangeKm { get; set; }
        public string? Status { get; set; }
        public List<AvailableLicensePlateDto> AvailableLicensePlates { get; set; } = new List<AvailableLicensePlateDto>();
        public decimal? EstimatedTotalCost { get; set; }
    }

    public class AvailableLicensePlateDto
    {
        public int LicensePlateId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string? Status { get; set; }
    }

    public class RentalSearchDto
    {
        public int? UserId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "OrderDate";
        public string? SortOrder { get; set; } = "desc";
    }

    public class RentalResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public RentalDto? Data { get; set; }
        public int? OrderId { get; set; }
        public int? ContractId { get; set; }
        public bool RequiresPayment { get; set; } = false;
    }

    public class RentalListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<RentalDto> Data { get; set; } = new List<RentalDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class AvailableVehicleListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<AvailableVehicleDto> Data { get; set; } = new List<AvailableVehicleDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
