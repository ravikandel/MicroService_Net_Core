
using AuthService.DTOs;
using AuthService.Models;

namespace AuthService.Repository
{
    public interface IAuthRepository
    {
        Task<bool> UpdateUserToken(User user, string accessToken);
        Task<User?> ValidateUser(LoginInputDto loginInputDto);
    }
}