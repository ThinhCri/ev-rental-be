using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;

namespace EV_RENTAL_SYSTEM.Models.DTOs
{
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
        public decimal? Battery { get; set; }
        public int? RangeKm { get; set; }
        public string? Status { get; set; }
        public int? StationId { get; set; }
        public string? StationName { get; set; }
        public string? StationStreet { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int AvailableLicensePlates { get; set; } = 0;
        public List<string> LicensePlateNumbers { get; set; } = new List<string>();
    }

    public class CreateVehicleDto
    {
        [Required(ErrorMessage = "Vehicle model name is required")]
        [MaxLength(100, ErrorMessage = "Model name cannot exceed 100 characters")]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2030, ErrorMessage = "Model year must be between 1900 and 2030")]
        public int? ModelYear { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public int BrandId { get; set; }


        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Daily rental price must be greater than 0")]
        public decimal? PricePerDay { get; set; }

        [Range(1, 50, ErrorMessage = "Seat number must be between 1 and 50")]
        public int? SeatNumber { get; set; }

        public IFormFile? VehicleImageFile { get; set; }

        [Range(0, 999.99, ErrorMessage = "Battery capacity must be between 0 and 999.99 kWh")]
        public decimal? Battery { get; set; }


        [Range(0, 9999, ErrorMessage = "Range must be between 0 and 9999 km")]
        public int? RangeKm { get; set; }

        public int? StationId { get; set; }

        [Required(ErrorMessage = "License plate number is required")]
        [MaxLength(50, ErrorMessage = "License plate number cannot exceed 50 characters")]
        public string LicensePlateNumber { get; set; } = string.Empty;
    }

    public class UpdateVehicleDto
    {
        [Required(ErrorMessage = "Vehicle model name is required")]
        [MaxLength(100, ErrorMessage = "Model name cannot exceed 100 characters")]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2030, ErrorMessage = "Model year must be between 1900 and 2030")]
        public int? ModelYear { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public int BrandId { get; set; }


        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Daily rental price must be greater than 0")]
        public decimal? PricePerDay { get; set; }

        [Range(1, 50, ErrorMessage = "Seat number must be between 1 and 50")]
        public int? SeatNumber { get; set; }

        public IFormFile? VehicleImageFile { get; set; }

        [Range(0, 999.99, ErrorMessage = "Battery capacity must be between 0 and 999.99 kWh")]
        public decimal? Battery { get; set; }


        [Range(0, 9999, ErrorMessage = "Range must be between 0 and 9999 km")]
        public int? RangeKm { get; set; }

        public int? StationId { get; set; }
    }

    public class VehicleResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public VehicleDto? Data { get; set; }
    }

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
        public string? SortBy { get; set; } = "Model"; 
        public string? SortOrder { get; set; } = "asc"; 
    }
}
