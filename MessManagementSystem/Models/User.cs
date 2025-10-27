using System.ComponentModel.DataAnnotations;

namespace Mess_Management_System.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }     // ✅ Primary key (required for EF)

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool Drink { get; set; }
        public bool Lunch { get; set; }

        public string Role { get; set; }
    }
}
