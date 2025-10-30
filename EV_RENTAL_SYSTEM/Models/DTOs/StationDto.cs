using System.ComponentModel.DataAnnotations;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
    public class StationDto
    {
        public int StationId { get; set; }
        public string? StationName { get; set; }
        public string? Street { get; set; }
        public string? District { get; set; }
        public string? Province { get; set; }
        public string? Country { get; set; }
        public string? FullAddress { get; set; }
        public int VehicleCount { get; set; }
        public int AvailableVehicleCount { get; set; }
    }

    public class CreateStationDto
    {
        [Required(ErrorMessage = "Station name is required")]
        [MaxLength(100, ErrorMessage = "Station name must not exceed 100 characters")]
        public string StationName { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Street name must not exceed 100 characters")]
        public string? Street { get; set; }

        [MaxLength(50, ErrorMessage = "District name must not exceed 50 characters")]
        public string? District { get; set; }

        [MaxLength(50, ErrorMessage = "Province name must not exceed 50 characters")]
        public string? Province { get; set; }

        [MaxLength(50, ErrorMessage = "Country name must not exceed 50 characters")]
        public string? Country { get; set; } = "Vietnam";
    }

    public class UpdateStationDto
    {
        [Required(ErrorMessage = "Station name is required")]
        [MaxLength(100, ErrorMessage = "Station name must not exceed 100 characters")]
        public string StationName { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Street name must not exceed 100 characters")]
        public string? Street { get; set; }

        [MaxLength(50, ErrorMessage = "District name must not exceed 50 characters")]
        public string? District { get; set; }

        [MaxLength(50, ErrorMessage = "Province name must not exceed 50 characters")]
        public string? Province { get; set; }

        [MaxLength(50, ErrorMessage = "Country name must not exceed 50 characters")]
        public string? Country { get; set; } = "Vietnam";
    }

    public class StationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public StationDto? Data { get; set; }
    }

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

    public class StationSearchDto
    {
        public string? StationName { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "StationName";
        public string? SortOrder { get; set; } = "asc";
    }
}
