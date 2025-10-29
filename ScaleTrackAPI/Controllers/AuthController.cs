using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.Services.Auth;
using ScaleTrackAPI.Services;

namespace ScaleTrackAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly UserService _userService;

        public AuthController(IAuthService auth, UserService userService)
        {
            _auth = auth;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (entity, error) = await _auth.LoginAsync(request);

            if (error is not null)
                return Unauthorized(new { error.Message });

            return Ok(entity);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (response, error, message) = await _userService.RegisterUser(request);

            if (error != null)
                return BadRequest(new { error.Message });

            return CreatedAtAction(nameof(Login), new { email = request.Email }, new
            {
                Data = response,
                Message = message
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var (entity, error) = await _auth.RefreshTokenAsync(request);

            if (error is not null)
                return Unauthorized(new { error.Message });

            return Ok(entity);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var error = await _auth.LogoutAsync(request);
            if (error is not null)
                return BadRequest(new { error.Message });

            return Ok();
        }
    }
}
