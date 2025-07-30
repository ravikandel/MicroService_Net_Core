using Microsoft.AspNetCore.Mvc;
using AuthService.Models;
using AuthService.DTOs;
using AuthService.Configurations.JWT;
using AuthService.Common;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Controllers
{
    [ApiVersion("1.0")]
    public class AuthController(IAuthLogic logic) : BaseController
    {
        private readonly IAuthLogic _logic = logic;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInputDto loginInputDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Username or Password are required!",
                });
            }

            var user = await _logic.ValidateUser(loginInputDto);
            if (user == null || string.IsNullOrEmpty(user.Password))
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Invalid Username or Password!"
                });
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginInputDto.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Incorrect Username or Password!"
                });
            }

            // create Token
            JwtConfig jwtConfig = new();
            var accessToken = jwtConfig.GetToken(user);

            // update Token 
            bool isSuccess = await _logic.UpdateUserToken(user, accessToken);

            if (!isSuccess)
            {
                return Ok(new Response
                {
                    StatusCode = EResult.Error,
                    Message = "Token creation failed. Try again!"
                });
            }

            return Ok(new Response<LoginResponseDto>
            {
                StatusCode = EResult.Success,
                Message = "Login Successful!",
                Data = new LoginResponseDto
                {
                    AccessToken = accessToken
                }
            });

        }
    }
}
