using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class LoginInputDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;

    }

}
