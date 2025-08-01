using AuthService.DTOs;
using AuthService.Models;
using AuthService.Repository;

namespace AuthService.Logic
{
    public class AuthLogic(IAuthRepository repository) : IAuthLogic
    {
        private readonly IAuthRepository _repository = repository;

        public async Task<User?> ValidateUser(LoginInputDto loginInputDto)
        {

            return await _repository.ValidateUser(loginInputDto);
        }

        public async Task<bool> UpdateUserToken(User user, string accessToken)
        {
            return await _repository.UpdateUserToken(user, accessToken);
        }

    }
}