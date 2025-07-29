using AuthService.Data;
using AuthService.Models;
using AuthService.Services.JWT;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public class UserService
    {
        private readonly AuthDbContext _authDbContext;

        public UserService(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;

            if (!_authDbContext.Users.Any())
            {
                var passwordHasher = new PasswordHasher<User>();

                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@admin.com",
                    Role = "admin",
                };

                // Hash the password
                adminUser.Password = passwordHasher.HashPassword(adminUser, "admin");

                // Save to DB
                _authDbContext.Users.Add(adminUser);
                _authDbContext.SaveChanges();
            }
        }

        public async Task<User?> ValidateUser(string username, string password)
        {

            var user = await _authDbContext.Users
                    .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);

            if (user == null)
                return null;

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password!, password);

            return result == PasswordVerificationResult.Success ? user : null;

        }

    }
}