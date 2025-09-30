using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Order_LicensePlate")]
    public class Order_LicensePlate
    {
        [Column("Order_Id")]
        public int OrderId { get; set; }

        [MaxLength(50)]
        [Column("License_plate_Id")]
        public string LicensePlateId { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("LicensePlateId")]
        public virtual LicensePlate LicensePlate { get; set; } = null!;
    }
}

