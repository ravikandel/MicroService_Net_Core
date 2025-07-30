using AuthService.DTOs;
using AuthService.Models;

public interface IAuthLogic
{
    Task<User?> ValidateUser(LoginInputDto loginInputDto);
    Task<bool> UpdateUserToken(User user, string accessToken);

}