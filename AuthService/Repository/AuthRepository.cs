using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.DTOs;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;


namespace AuthService.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDbContext _context;

        public AuthRepository(AuthDbContext context)
        {
            _context = context;

            if (!_context.Users.Any())
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
                _context.Users.Add(adminUser);
                _context.SaveChanges();
            }
        }
        public async Task<User?> ValidateUser(LoginInputDto loginInputDto)
        {
            var usernameOrEmail = loginInputDto.Username.Trim().ToLower();

            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username!.ToLower() == usernameOrEmail || u.Email!.ToLower() == usernameOrEmail);
        }


        public async Task<bool> UpdateUserToken(User user, string accessToken)
        {
            try
            {
                user.AccessToken = accessToken;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}