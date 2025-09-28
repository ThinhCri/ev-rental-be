using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [Column("Order_Id")]
        public int OrderId { get; set; }

        [Column("Order_date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column("Start_time")]
        public DateTime? StartTime { get; set; }

        [Column("End_time")]
        public DateTime? EndTime { get; set; }

        [Column("Total_amount", TypeName = "decimal(10,2)")]
        public decimal? TotalAmount { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [Column("User_Id")]
        public int UserId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<Order_LicensePlate> OrderLicensePlates { get; set; } = new List<Order_LicensePlate>();
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}


