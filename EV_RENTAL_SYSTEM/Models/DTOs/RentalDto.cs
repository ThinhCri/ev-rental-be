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
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
        public decimal? Deposit { get; set; }
        public decimal? RentalFee { get; set; }
        public decimal? ExtraFee { get; set; }
        public List<RentalPaymentDto> Payments { get; set; } = new List<RentalPaymentDto>();
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

    /// <summary>
    /// DTO cho request tạo đơn thuê xe
    /// </summary>
    public class CreateRentalDto
    {
        [Required(ErrorMessage = "Thời gian bắt đầu thuê là bắt buộc")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc thuê là bắt buộc")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Danh sách xe thuê là bắt buộc")]
        [MinLength(1, ErrorMessage = "Phải chọn ít nhất 1 xe")]
        public List<int> VehicleIds { get; set; } = new List<int>();

        [Range(0, double.MaxValue, ErrorMessage = "Phí cọc phải lớn hơn hoặc bằng 0")]
        public decimal? DepositAmount { get; set; }

        [MaxLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        public string? Notes { get; set; }
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

