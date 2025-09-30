using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        [Column("Payment_Id")]
        public int PaymentId { get; set; }

        [Column("Payment_date")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Amount { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [Column("Contract_Id")]
        public int ContractId { get; set; }

        // Navigation properties
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; } = null!;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}

