using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        [Column("User_Id")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("Full_name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

		[Column("Birthday")]
		public DateTime? Birthday { get; set; }

        [Column("Created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? Status { get; set; }

        public string Phone { get; set; } 

        [Column("Role_Id")]
        public int RoleId { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;

        public virtual ICollection<License> Licenses { get; set; } = new List<License>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
