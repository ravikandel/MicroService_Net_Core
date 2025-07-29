using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using AuthService.Models;
using AuthService.Services;
using AuthService.Services.JWT;

namespace AuthService.Controllers
{
    [ApiVersion("1.0")]
    public class AuthController(UserService userService, AuthDbContext context) : BaseController
    {
        private readonly UserService _userService = userService;
        private readonly AuthDbContext _context = context;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInputDto loginInputDto)
        {
            var user = await _userService.ValidateUser(loginInputDto.Username, loginInputDto.Password);
            if (user == null) return Ok(new Response
            {
                StatusCode = EResult.Error,
                Message = "Invalid Username or Password!"
            });

            JwtConfig jwtConfig = new();
            var accessToken = jwtConfig.GetToken(user);
            user.AccessToken = accessToken;

            await _context.SaveChangesAsync();

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
