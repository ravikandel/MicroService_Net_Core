namespace AuthService.Services.JWT
{
    public class JwtConfigModel
    {
        public int ExpirationInMinutes { get; set; }
        public required string Key { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
    }
}