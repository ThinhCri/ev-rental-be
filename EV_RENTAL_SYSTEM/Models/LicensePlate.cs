using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EV_RENTAL_SYSTEM.Models.Enums;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("LicensePlate")]
    public class LicensePlate
    {
        [Key]
        [Column("License_plate_Id")]
        public int LicensePlateId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Plate_Number")]
        public string PlateNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Status { get; set; } // Trạng thái biển số (Available, Rented, Maintenance)

        [Column("Vehicle_Id")]
        public int VehicleId { get; set; }

        [Column("Registration_date", TypeName = "date")]
        public DateTime? RegistrationDate { get; set; }

        [Column("Station_Id")]
        public int StationId { get; set; }

        // Navigation properties
        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [ForeignKey("StationId")]
        public virtual Station Station { get; set; } = null!;

        public virtual ICollection<Order_LicensePlate> OrderLicensePlates { get; set; } = new List<Order_LicensePlate>();
        public virtual ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
    }
}


