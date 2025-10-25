using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Transaction")]
    public class Transaction
    {
        [Key]
        [Column("Transaction_Id")]
        public int TransactionId { get; set; }

        [Column("Transaction_date")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Column("Payment_Id")]
        public int PaymentId { get; set; }

        [Column("User_Id")]
        public int UserId { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        // Navigation properties
        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}


