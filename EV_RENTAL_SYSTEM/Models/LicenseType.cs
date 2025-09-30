using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("LicenseType")]
    public class LicenseType
    {
        [Key]
        [Column("License_type_Id")]
        public int LicenseTypeId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Type_name")]
        public string TypeName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        // Navigation properties
        public virtual ICollection<License> Licenses { get; set; } = new List<License>();
    }
}

