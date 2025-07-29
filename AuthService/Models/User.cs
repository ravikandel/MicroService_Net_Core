using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string? Username { get; set; } 
        
        [Required]
        public string? Email { get; set; } 

        [Required]
        public string? Password { get; set; } 

        public string? AccessToken { get; set; } 

        [Required]
        public string Role { get; set; } = "User";

    }
}