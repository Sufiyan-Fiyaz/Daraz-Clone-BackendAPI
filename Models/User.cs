using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daraz_CloneAgain.Models
{
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Column("full_name")]
        public string FullName { get; set; }

        [Required, MaxLength(150)]
        [Column("email")]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        [Column("password")]
        public string Password { get; set; }

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }



        [Required, MaxLength(20)]
        [Column("role")]
        public string Role { get; set; } = "customer";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


    }
}
