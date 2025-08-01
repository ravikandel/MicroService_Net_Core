using AuthService.DTOs;
using AuthService.Models;

namespace AuthService.Logic
{
    public interface IAuthLogic
    {
        Task<User?> ValidateUser(LoginInputDto loginInputDto);
        Task<bool> UpdateUserToken(User user, string accessToken);

    }
}