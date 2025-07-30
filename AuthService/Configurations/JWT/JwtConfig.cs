using AuthService.Models;

namespace AuthService.Configurations.JWT
{
    public class JwtConfig
    {
        private IConfigurationRoot? Configuration { get; set; }

        public string GetToken(User user)
        {
            IConfigurationBuilder configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            Configuration = configuration.Build();
            JwtConfigModel app = new()
            {
                ExpirationInMinutes = int.Parse(Configuration.GetSection("JwtConfig:ExpirationInMinutes").Value!),
                Key = Configuration.GetSection("JwtConfig:Key").Value!.ToLower(),
                Issuer = Configuration.GetSection("JwtConfig:Issuer").Value!.ToLower(),
                Audience = Configuration.GetSection("JwtConfig:Audience").Value!.ToLower(),
            };

            var token = new JwtTokenBuilder()
             .AddSecurityKey(JwtSecurityKey.Create(app.Key))
             .AddSubject(user.Username!.ToLower().ToString())
             .AddIssuer(app.Issuer)
             .AddAudience(app.Audience)
             .AddClaim("Role", user.Role) // can add extra claims
             .AddExpiry(app.ExpirationInMinutes)
             .Build();

            return token.Value;

        }
    }
}