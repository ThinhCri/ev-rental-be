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

        [MaxLength(510)]
        public string? Description { get; set; }

        [Column("Seat_number")]
        public int? SeatNumber { get; set; }

        [MaxLength(255)]
        [Column("vehicle_img")]
        public string? VehicleImage { get; set; }

        [Column("price_per_day")]
        public int? PricePerDay { get; set; }

        [MaxLength(10)]
        public string? Battery { get; set; }

        // Navigation properties
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; } = null!;

        public virtual ICollection<LicensePlate> LicensePlates { get; set; } = new List<LicensePlate>();
    }
}

