using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    /// <summary>
    /// DTO cho thông tin đơn thuê xe (trả về cho client)
    /// </summary>
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
        
        // Thêm thông tin bằng lái xe
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

    /// <summary>
    /// DTO cho thông tin xe trong đơn thuê
    /// </summary>
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

    /// <summary>
    /// DTO cho thông tin biển số trong đơn thuê
    /// </summary>
    public class RentalLicensePlateDto
    {
        public int LicensePlateId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string? Status { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin hợp đồng trong đơn thuê
    /// </summary>
    public class RentalContractDto
    {
        public int ContractId { get; set; }
        public string? ContractCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
        public decimal? Deposit { get; set; }
        public decimal? RentalFee { get; set; }
        public decimal? ExtraFee { get; set; }
        public List<RentalPaymentDto> Payments { get; set; } = new List<RentalPaymentDto>();
    }

    /// <summary>
    /// DTO cho bảng hợp đồng hiển thị các loại phí
    /// </summary>
    public class ContractSummaryDto
    {
        public string ContractCode { get; set; } = string.Empty;
        public decimal RentalFee { get; set; }
        public decimal Deposit { get; set; }
        public decimal OverKmFee { get; set; } = 0;
        public decimal ElectricityFee { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Paid, Cancelled
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiryDate { get; set; } // Thời gian hết hạn QR code (2 phút)
        public List<ContractFeeDetailDto> FeeDetails { get; set; } = new List<ContractFeeDetailDto>();
    }

    /// <summary>
    /// DTO cho chi tiết các loại phí trong hợp đồng
    /// </summary>
    public class ContractFeeDetailDto
    {
        public string FeeType { get; set; } = string.Empty; // "Rental", "Deposit", "OverKm", "Electricity"
        public string FeeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cho request xác nhận của staff
    /// </summary>
    public class StaffConfirmationDto
    {
        [Required(ErrorMessage = "Trạng thái xác nhận là bắt buộc")]
        public bool IsConfirmed { get; set; }
        
        [MaxLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        public string? Notes { get; set; }
        
        public string Action { get; set; } = string.Empty; // "Handover", "Return"
    }

    /// <summary>
    /// DTO cho request bàn giao xe với thông tin chi tiết
    /// </summary>
    public class HandoverVehicleDto
    {
        [Required(ErrorMessage = "Hình ảnh xe là bắt buộc")]
        public IFormFile? VehicleImage { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Ghi chú không được quá 1000 ký tự")]
        public string? Notes { get; set; }
        
        [Required(ErrorMessage = "Odometer là bắt buộc")]
        [Range(0, 999999, ErrorMessage = "Odometer phải từ 0 đến 999999 km")]
        public int Odometer { get; set; } // Số km hiện tại của xe
        
        [Required(ErrorMessage = "Pin là bắt buộc")]
        [Range(0, 100, ErrorMessage = "Pin phải từ 0 đến 100%")]
        public decimal Battery { get; set; } // % pin hiện tại
    }

    /// <summary>
    /// DTO cho request trả xe với thông tin chi tiết
    /// </summary>
    public class ReturnVehicleDto
    {
        [Required(ErrorMessage = "Hình ảnh xe là bắt buộc")]
        public IFormFile? VehicleImage { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Ghi chú không được quá 1000 ký tự")]
        public string? Notes { get; set; }
        
        [Required(ErrorMessage = "Số km là bắt buộc")]
        [Range(0, 999999, ErrorMessage = "Số km phải từ 0 đến 999999 km")]
        public int Odometer { get; set; } // Số km sau khi trả xe
        
        [Required(ErrorMessage = "Pin là bắt buộc")]
        [Range(0, 100, ErrorMessage = "Pin phải từ 0 đến 100%")]
        public decimal Battery { get; set; } // % pin khi trả xe
    }

    /// <summary>
    /// DTO cho response QR code thanh toán
    /// </summary>
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

    /// <summary>
    /// DTO cho thông tin thanh toán trong đơn thuê
    /// </summary>
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
       
        [Required(ErrorMessage = "Thời gian bắt đầu thuê là bắt buộc")]
        public DateTime StartTime { get; set; }

      
        [Required(ErrorMessage = "Thời gian kết thúc thuê là bắt buộc")]
        public DateTime EndTime { get; set; }

  
        [Required(ErrorMessage = "ID xe thuê là bắt buộc")]
        public int VehicleId { get; set; }

    
        [Range(0, double.MaxValue, ErrorMessage = "Phí cọc phải lớn hơn hoặc bằng 0")]
        public decimal? DepositAmount { get; set; }

        public bool IsBookingForOthers { get; set; } = false;

        public IFormFile? RenterLicenseImage { get; set; }
    }

    /// <summary>
    /// DTO cho request cập nhật đơn thuê
    /// </summary>
    public class UpdateRentalDto
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<int>? VehicleIds { get; set; }
        public decimal? DepositAmount { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        
        public IFormFile? RenterLicenseImage { get; set; } // Hỗ trợ upload ảnh bằng lái xe nếu đặt hộ
    }

    /// <summary>
    /// DTO cho request tìm kiếm xe có sẵn
    /// </summary>
    public class AvailableVehiclesSearchDto
    {
        [Required(ErrorMessage = "Thời gian bắt đầu là bắt buộc")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc là bắt buộc")]
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

    /// <summary>
    /// DTO cho thông tin xe có sẵn
    /// </summary>
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

    /// <summary>
    /// DTO cho biển số có sẵn
    /// </summary>
    public class AvailableLicensePlateDto
    {
        public int LicensePlateId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string? Status { get; set; }
    }

    /// <summary>
    /// DTO cho request tìm kiếm đơn thuê
    /// </summary>
    public class RentalSearchDto
    {
        public int? UserId { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "OrderDate"; // OrderDate, TotalAmount, Status
        public string? SortOrder { get; set; } = "desc"; // asc, desc
    }

    /// <summary>
    /// DTO cho response API
    /// </summary>
    public class RentalResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public RentalDto? Data { get; set; }
        public int? OrderId { get; set; } // Thêm OrderId để API thanh toán có thể sử dụng
        public int? ContractId { get; set; } // Thêm ContractId để API thanh toán có thể sử dụng
        public bool RequiresPayment { get; set; } = false; // Flag để frontend biết cần thanh toán
    }

    /// <summary>
    /// DTO cho danh sách đơn thuê
    /// </summary>
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

    /// <summary>
    /// DTO cho danh sách xe có sẵn
    /// </summary>
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

