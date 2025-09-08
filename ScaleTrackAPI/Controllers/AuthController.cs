using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.Services;
using ScaleTrackAPI.Services.Auth;

namespace ScaleTrackAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(IAuthService auth, UserService userService) : ControllerBase
    {
        private readonly IAuthService _auth = auth;
        private readonly UserService _userService = userService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _auth.LoginAsync(request);
            if (result == null) return Unauthorized(new { message = "Invalid email or password." });
            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (response, error) = await _userService.RegisterUser(request);
            if (error != null)
            {
                return error.Code switch
                {
                    "ValidationError" => BadRequest(error),
                    "Conflict" => Conflict(error),
                    _ => BadRequest(error)
                };
            }

            return CreatedAtAction(nameof(Login), new { email = request.Email }, response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var result = await _auth.RefreshTokenAsync(request);
            if (result == null) return Unauthorized(new { message = "Invalid refresh token" });
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var success = await _auth.LogoutAsync(request);
            return Ok(new { success });
        }
    }

}
