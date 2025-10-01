using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("ProcessStep")]
    public class ProcessStep
    {
        [Key]
        [Column("Step_Id")]
        public int StepId { get; set; }

        [MaxLength(50)]
        [Column("Step_name")]
        public string? StepName { get; set; }

        [MaxLength(255)]
        public string? Terms { get; set; }

        public int? Step_order { get; set; }

        // Navigation properties
        public virtual ICollection<ContractProcessing> ContractProcessings { get; set; } = new List<ContractProcessing>();
    }
}

