
using AuthService.DTOs;
using AuthService.Models;

public interface IAuthRepository
{
    Task<bool> UpdateUserToken(User user, string accessToken);
    Task<User?> ValidateUser(LoginInputDto loginInputDto);
}