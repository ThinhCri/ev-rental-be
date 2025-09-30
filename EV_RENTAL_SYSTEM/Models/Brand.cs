using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Brand")]
    public class Brand
    {
        [Key]
        [Column("Brand_Id")]
        public int BrandId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Brand_name")]
        public string BrandName { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}

