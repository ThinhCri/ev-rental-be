using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("License")]
    public class License
    {
        [Key]
        [Column("License_Id")]
        public int LicenseId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("License_number")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        [Column("Expiry_date", TypeName = "date")]
        public DateTime ExpiryDate { get; set; }

        [Column("User_Id")]
        public int UserId { get; set; }

        [Column("License_type_Id")]
        public int LicenseTypeId { get; set; }

        [MaxLength(255)]
        [Column("License_ImageUrl")]
        public string? LicenseImageUrl { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("LicenseTypeId")]
        public virtual LicenseType LicenseType { get; set; } = null!;
    }
}

