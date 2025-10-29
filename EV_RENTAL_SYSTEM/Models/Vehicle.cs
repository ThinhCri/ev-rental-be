using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EV_RENTAL_SYSTEM.Models.Enums;

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

        [Column("Range_km")]
        public int? RangeKm { get; set; } // Tầm hoạt động (km)

        // Navigation properties
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; } = null!;

        public virtual ICollection<LicensePlate> LicensePlates { get; set; } = new List<LicensePlate>();
    }
}


