using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Station")]
    public class Station
    {
        [Key]
        [Column("Station_Id")]
        public int StationId { get; set; }

        [MaxLength(100)]
        [Column("Station_name")]
        public string? StationName { get; set; }

        [MaxLength(100)]
        public string? Street { get; set; }

        [MaxLength(50)]
        public string? District { get; set; }

        [MaxLength(50)]
        public string? Province { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }

        public int? Total_vehicle { get; set; }

        [MaxLength(100)]
        public string? Ward { get; set; }

        // Navigation properties
        public virtual ICollection<LicensePlate> LicensePlates { get; set; } = new List<LicensePlate>();
    }
}

