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

        [Column("Total_Vehicle")]
        public int TotalVehicle { get; set; }

        [Column("Available_Vehicle")]
        public int AvailableVehicle { get; set; }

        public virtual ICollection<LicensePlate> LicensePlates { get; set; } = new List<LicensePlate>();
        public virtual ICollection<User> Staff { get; set; } = new List<User>();
    }
}
