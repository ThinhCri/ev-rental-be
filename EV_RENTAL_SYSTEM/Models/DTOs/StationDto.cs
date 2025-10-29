using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    /// <summary>
    /// DTO cho thông tin trạm xe (trả về cho client)
    /// </summary>
    public class StationDto
    {
        public int StationId { get; set; }
        public string? StationName { get; set; }
        public string? Street { get; set; }
        public string? District { get; set; }
        public string? Province { get; set; }
        public string? Country { get; set; }
        public string? FullAddress { get; set; } // Địa chỉ đầy đủ
        public int VehicleCount { get; set; } // Số lượng xe trong trạm
        public int AvailableVehicleCount { get; set; } // Số xe có sẵn
    }

    /// <summary>
    /// DTO cho request tạo trạm mới
    /// </summary>
    public class CreateStationDto
    {
        [Required(ErrorMessage = "Tên trạm là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên trạm không được quá 100 ký tự")]
        public string StationName { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Tên đường không được quá 100 ký tự")]
        public string? Street { get; set; }

        [MaxLength(50, ErrorMessage = "Quận/Huyện không được quá 50 ký tự")]
        public string? District { get; set; }

        [MaxLength(50, ErrorMessage = "Tỉnh/Thành phố không được quá 50 ký tự")]
        public string? Province { get; set; }

        [MaxLength(50, ErrorMessage = "Quốc gia không được quá 50 ký tự")]
        public string? Country { get; set; } = "Vietnam";
    }

    /// <summary>
    /// DTO cho request cập nhật trạm
    /// </summary>
    public class UpdateStationDto
    {
        [Required(ErrorMessage = "Tên trạm là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên trạm không được quá 100 ký tự")]
        public string StationName { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Tên đường không được quá 100 ký tự")]
        public string? Street { get; set; }

        [MaxLength(50, ErrorMessage = "Quận/Huyện không được quá 50 ký tự")]
        public string? District { get; set; }

        [MaxLength(50, ErrorMessage = "Tỉnh/Thành phố không được quá 50 ký tự")]
        public string? Province { get; set; }

        [MaxLength(50, ErrorMessage = "Quốc gia không được quá 50 ký tự")]
        public string? Country { get; set; } = "Vietnam";
    }

    /// <summary>
    /// DTO cho response API
    /// </summary>
    public class StationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public StationDto? Data { get; set; }
    }

    /// <summary>
    /// DTO cho danh sách trạm
    /// </summary>
    public class StationListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<StationDto> Data { get; set; } = new List<StationDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// DTO cho tìm kiếm trạm
    /// </summary>
    public class StationSearchDto
    {
        public string? StationName { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "StationName"; // StationName, Province, District
        public string? SortOrder { get; set; } = "asc"; // asc, desc
    }
}

