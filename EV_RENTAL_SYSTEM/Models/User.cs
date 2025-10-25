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

        [Required]
        [MaxLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Column("Phone_number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime? Birthday { get; set; }

        [Column("Created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? Status { get; set; }

        [Column("Role_Id")]
        public int RoleId { get; set; }

        [Column("Station_Id")]
        public int? StationId { get; set; } // Trạm mà staff quản lý (chỉ dành cho Station Staff)

        [MaxLength(500)]
        public string? Notes { get; set; } // Ghi chú về staff

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;

        [ForeignKey("StationId")]
        public virtual Station? Station { get; set; }

        public virtual ICollection<License> Licenses { get; set; } = new List<License>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
