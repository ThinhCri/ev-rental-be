using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("ContractProcessing")]
    public class ContractProcessing
    {
        [Key]
        [Column("ContractProcessing_Id")]
        public int ContractProcessingId { get; set; }

        [Column("Contract_Id")]
        public int ContractId { get; set; }

        [Column("Step_Id")]
        public int StepId { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        // Navigation properties
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; } = null!;

        [ForeignKey("StepId")]
        public virtual ProcessStep ProcessStep { get; set; } = null!;
    }
}

