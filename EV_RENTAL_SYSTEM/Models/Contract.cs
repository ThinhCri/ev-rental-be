using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Contract")]
    public class Contract
    {
        [Key]
        [Column("Contract_Id")]
        public int ContractId { get; set; }

        [Column("Order_Id")]
        public int OrderId { get; set; }

        [Column("Created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? Status { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Deposit { get; set; }

        [Column("Rental_fee", TypeName = "decimal(10,2)")]
        public decimal? RentalFee { get; set; }

        [Column("Extra_fee", TypeName = "decimal(10,2)")]
        public decimal? ExtraFee { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        public virtual ICollection<ContractProcessing> ContractProcessings { get; set; } = new List<ContractProcessing>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}

