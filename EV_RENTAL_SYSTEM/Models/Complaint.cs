using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Complaint")]
    public class Complaint
    {
        [Key]
        [Column("Complaint_Id")]
        public int ComplaintId { get; set; }

        [Column("Complaint_date")]
        public DateTime ComplaintDate { get; set; } = DateTime.Now;

        [MaxLength(255)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [Column("User_Id")]
        public int UserId { get; set; }

        [Column("Order_Id")]
        public int OrderId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}


