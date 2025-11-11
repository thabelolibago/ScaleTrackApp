using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Login;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Token;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Services;
using ScaleTrackAPI.Application.Features.RegisterUser;

namespace ScaleTrackAPI.Controllers.Auth.AuthController
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RegisterUserService _userService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, RegisterUserService userService, IConfiguration config)
        {
            _authService = authService;
            _userService = userService;
            _config = config;
        }

        // 🔹 Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (entity, error) = await _authService.LoginAsync(request, User);
            if (error is not null) return Unauthorized(error);
            return Ok(entity);
        }

        // 🔹 Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            string baseUrl = _config["App:FrontendUrl"] ?? "http://localhost:4200";
            var (response, error) = await _userService.RegisterUser(request, baseUrl);

            if (error != null) return BadRequest(new { error.Message });

            return CreatedAtAction(nameof(Register), response);
        }

        // 🔹 Verify email
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var error = await _authService.VerifyEmailAsync(token);
            if (error != null) return BadRequest(error);
            return Ok();
        }

        // 🔹 Resend verification email
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromQuery] string email)
        {
            string baseUrl = _config["App:FrontendUrl"] ?? "http://localhost:4200";
            var error = await _authService.ResendVerificationEmailAsync(email, baseUrl);
            if (error != null) return StatusCode(error.StatusCode, error);
            return Ok();
        }

        // 🔹 Refresh token
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var (entity, error) = await _authService.RefreshTokenAsync(request, User);
            if (error is not null) return Unauthorized(error);
            return Ok(entity);
        }

        // 🔹 Logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var error = await _authService.LogoutAsync(request, User);
            if (error is not null) return BadRequest(error);
            return Ok();
        }
    }
}

