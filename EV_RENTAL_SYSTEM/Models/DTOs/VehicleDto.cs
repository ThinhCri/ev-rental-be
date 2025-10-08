using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    /// <summary>
    /// DTO cho thông tin xe (trả về cho client)
    /// </summary>
    public class VehicleDto
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = string.Empty;
        public int? ModelYear { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? PricePerDay { get; set; }
        public int? SeatNumber { get; set; }
        public string? VehicleImage { get; set; }
        public decimal? Battery { get; set; } // Dung lượng pin (kWh)
        public int? RangeKm { get; set; } // Tầm hoạt động (km)
        public string? Status { get; set; } // Trạng thái xe
        public int? StationId { get; set; } // ID trạm xe đang đậu
        public string? StationName { get; set; } // Tên trạm xe đang đậu
        public string? StationStreet { get; set; } // Địa chỉ trạm xe (cho Google Maps)
        public bool IsAvailable { get; set; } = true; // Trạng thái có sẵn để thuê
        public int AvailableLicensePlates { get; set; } = 0; // Số biển số có sẵn
        public List<string> LicensePlateNumbers { get; set; } = new List<string>(); // Danh sách biển số xe
    }

    /// <summary>
    /// DTO cho request tạo xe mới
    /// </summary>
    public class CreateVehicleDto
    {
        [Required(ErrorMessage = "Tên model xe là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên model không được quá 100 ký tự")]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2030, ErrorMessage = "Năm sản xuất phải từ 1900 đến 2030")]
        public int? ModelYear { get; set; }

        [Required(ErrorMessage = "Thương hiệu là bắt buộc")]
        public int BrandId { get; set; }


        [MaxLength(255, ErrorMessage = "Mô tả không được quá 255 ký tự")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá thuê hàng ngày phải lớn hơn 0")]
        public decimal? PricePerDay { get; set; }

        [Range(1, 50, ErrorMessage = "Số ghế phải từ 1 đến 50")]
        public int? SeatNumber { get; set; }

        public IFormFile? VehicleImageFile { get; set; }

        [Range(0, 999.99, ErrorMessage = "Dung lượng pin phải từ 0 đến 999.99 kWh")]
        public decimal? Battery { get; set; }


        [Range(0, 9999, ErrorMessage = "Tầm hoạt động phải từ 0 đến 9999 km")]
        public int? RangeKm { get; set; }

        [MaxLength(50, ErrorMessage = "Trạng thái không được quá 50 ký tự")]
        public string? Status { get; set; }

        public int? StationId { get; set; } // ID trạm xe đang đậu

        [Required(ErrorMessage = "Biển số xe là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Biển số xe không được quá 50 ký tự")]
        public string LicensePlateNumber { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Tỉnh thành không được quá 50 ký tự")]
        public string? Province { get; set; }

        [MaxLength(50, ErrorMessage = "Tình trạng biển số không được quá 50 ký tự")]
        public string? LicensePlateCondition { get; set; } = "Good";
    }

    /// <summary>
    /// DTO cho request cập nhật xe
    /// </summary>
    public class UpdateVehicleDto
    {
        [Required(ErrorMessage = "Tên model xe là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên model không được quá 100 ký tự")]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2030, ErrorMessage = "Năm sản xuất phải từ 1900 đến 2030")]
        public int? ModelYear { get; set; }

        [Required(ErrorMessage = "Thương hiệu là bắt buộc")]
        public int BrandId { get; set; }


        [MaxLength(255, ErrorMessage = "Mô tả không được quá 255 ký tự")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá thuê hàng ngày phải lớn hơn 0")]
        public decimal? PricePerDay { get; set; }

        [Range(1, 50, ErrorMessage = "Số ghế phải từ 1 đến 50")]
        public int? SeatNumber { get; set; }

        public IFormFile? VehicleImageFile { get; set; }

        [Range(0, 999.99, ErrorMessage = "Dung lượng pin phải từ 0 đến 999.99 kWh")]
        public decimal? Battery { get; set; }


        [Range(0, 9999, ErrorMessage = "Tầm hoạt động phải từ 0 đến 9999 km")]
        public int? RangeKm { get; set; }

        [MaxLength(50, ErrorMessage = "Trạng thái không được quá 50 ký tự")]
        public string? Status { get; set; }

        public int? StationId { get; set; } // ID trạm xe đang đậu
    }

    /// <summary>
    /// DTO cho response API
    /// </summary>
    public class VehicleResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public VehicleDto? Data { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách xe
    /// </summary>
    public class VehicleListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<VehicleDto> Data { get; set; } = new List<VehicleDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// DTO cho tìm kiếm xe
    /// </summary>
    public class VehicleSearchDto
    {
        public string? Model { get; set; }
        public int? BrandId { get; set; }
        public decimal? MinPricePerDay { get; set; }
        public decimal? MaxPricePerDay { get; set; }
        public int? MinSeatNumber { get; set; }
        public int? MaxSeatNumber { get; set; }
        public bool? IsAvailable { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "Model"; // Model, DailyRate, ModelYear
        public string? SortOrder { get; set; } = "asc"; // asc, desc
    }
}
