using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("LicensePlate")]
    public class LicensePlate
    {
        [Key]
        [MaxLength(50)]
        [Column("License_plate_Id")]
        public string LicensePlateId { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Status { get; set; }

        [Column("Vehicle_Id")]
        public int VehicleId { get; set; }

        [MaxLength(50)]
        public string? Province { get; set; }

        [Column("Registration_date")]
        public DateTime? RegistrationDate { get; set; }

        [MaxLength(50)]
        public string? Condition { get; set; }

        [Column("Station_Id")]
        public int StationId { get; set; }

        [Column("Kilometers_driven", TypeName = "decimal(10,2)")]
        public decimal? KilometersDriven { get; set; }

        [MaxLength(50)]
        [Column("Plate_number", TypeName = "varchar(50)")]
        public string? PlateNumber { get; set; }

        // Navigation properties
        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [ForeignKey("StationId")]
        public virtual Station Station { get; set; } = null!;

        public virtual ICollection<Order_LicensePlate> OrderLicensePlates { get; set; } = new List<Order_LicensePlate>();
        public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
    }
}

