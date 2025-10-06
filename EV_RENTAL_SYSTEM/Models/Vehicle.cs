using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Vehicle")]
    public class Vehicle
    {
        [Key]
        [Column("Vehicle_Id")]
        public int VehicleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Column("Model_year")]
        public int? ModelYear { get; set; }

        [Column("Brand_Id")]
        public int BrandId { get; set; }

        [MaxLength(50)]
        [Column("Vehicle_type")]
        public string? VehicleType { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [Column("Price_per_day", TypeName = "decimal(10,2)")]
        public decimal? PricePerDay { get; set; }

        [Column("Seat_number")]
        public int? SeatNumber { get; set; }

        [MaxLength(500)]
        [Column("Vehicle_image")]
        public string? VehicleImage { get; set; }

        [Column("Battery", TypeName = "decimal(5,2)")]
        public decimal? Battery { get; set; } // Dung lượng pin (kWh)

        [Column("Charging_time", TypeName = "decimal(4,1)")]
        public decimal? ChargingTime { get; set; } // Thời gian sạc (giờ)

        [Column("Range_km")]
        public int? RangeKm { get; set; } // Tầm hoạt động (km)

        [MaxLength(50)]
        [Column("Status")]
        public string? Status { get; set; } // Trạng thái xe (Good, Maintenance, etc.)

        [Column("Station_Id")]
        public int? StationId { get; set; } // Trạm xe đang đậu

        // Navigation properties
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; } = null!;

        [ForeignKey("StationId")]
        public virtual Station? Station { get; set; }

        public virtual ICollection<LicensePlate> LicensePlates { get; set; } = new List<LicensePlate>();
    }
}


