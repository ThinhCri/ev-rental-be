using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Maintenance")]
    public class Maintenance
    {
        [Key]
        [Column("Maintenance_Id")]
        public int MaintenanceId { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Cost { get; set; }

        [Column("Maintenance_date", TypeName = "date")]
        public DateTime? MaintenanceDate { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [Column("License_plate_Id")]
        public int LicensePlateId { get; set; }

        // Navigation properties
        [ForeignKey("LicensePlateId")]
        public virtual LicensePlate LicensePlate { get; set; } = null!;
    }
}


