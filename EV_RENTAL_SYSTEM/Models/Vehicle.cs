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

        [Column("Daily_rate", TypeName = "decimal(10,2)")]
        public decimal? DailyRate { get; set; }

        [Column("Seat_number")]
        public int? SeatNumber { get; set; }

        // Navigation properties
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; } = null!;

        public virtual ICollection<LicensePlate> LicensePlates { get; set; } = new List<LicensePlate>();
    }
}


