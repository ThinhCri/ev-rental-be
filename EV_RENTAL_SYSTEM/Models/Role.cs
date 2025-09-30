using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EV_RENTAL_SYSTEM.Models
{
    [Table("Role")]
    public class Role
    {
        [Key]
        [Column("Role_Id")]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Role_name")]
        public string RoleName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}

