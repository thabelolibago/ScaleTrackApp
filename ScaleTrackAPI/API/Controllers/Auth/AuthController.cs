using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Application.Features.Auth.Login.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Login.Services;
using ScaleTrackAPI.Application.Features.Auth.Logout.DTOs;
using ScaleTrackAPI.Application.Features.Auth.Logout.Services;
using ScaleTrackAPI.Application.Features.Auth.Refresh.DTOs;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs;
using ScaleTrackAPI.Application.Features.Auth.ResendVerification.Services;
using ScaleTrackAPI.Application.Features.Auth.Services;
using ScaleTrackAPI.Application.Features.Auth.VerifyEmail.Services;
using ScaleTrackAPI.Application.Features.RegisterUser;

namespace ScaleTrackAPI.Controllers.Auth.AuthController
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly RegisterUserService _userService;
        private readonly ILoginService _loginService;
        private readonly ILogoutService _logoutService;
        private readonly IVerifyEmailService _verifyEmailService;
        private readonly IResendVerificationService _resendVerificationService;
        private readonly IConfiguration _config;

        public AuthController(
            IAuthService authService,
            ILoginService loginService,
            ILogoutService logoutService,
            RegisterUserService userService,
            IVerifyEmailService verifyEmailService,
            IResendVerificationService resendVerificationService,
            IConfiguration config)
        {
            _authService = authService;
            _loginService = loginService;
            _logoutService = logoutService;
            _userService = userService;
            _verifyEmailService = verifyEmailService;
            _resendVerificationService = resendVerificationService;
            _config = config;
        }

        #region Authentication

        /// <summary>
        /// Logs in a user and returns access & refresh tokens.
        /// POST api/v1/auth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (entity, error) = await _loginService.LoginAsync(request, User);
            if (error is not null) return Unauthorized(error);
            return Ok(entity);
        }

        /// <summary>
        /// Registers a new user and sends verification email.
        /// POST api/v1/auth/register
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            // Frontend URL for email verification link
            string baseUrl = _config["App:FrontendUrl"] ?? "http://localhost:4200";
            var (response, error) = await _userService.RegisterUser(request, baseUrl);

            if (error != null) return BadRequest(new { error.Message });

            return CreatedAtAction(nameof(Register), response);
        }

        /// <summary>
        /// Verifies a user's email with a token.
        /// GET api/v1/auth/verify-email?token=
        /// </summary>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var error = await _verifyEmailService.VerifyEmailAsync(token);
            if (error != null) return BadRequest(error);
            return Ok();
        }

        /// <summary>
        /// Resends the verification email.
        /// POST api/v1/auth/resend-verification?email=
        /// </summary>
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromQuery] string email)
        {
            string baseUrl = _config["App:FrontendUrl"] ?? "http://localhost:4200";
            var error = await _resendVerificationService.ResendVerificationEmailAsync(email, baseUrl);
            if (error != null) return StatusCode(error.StatusCode, error);
            return Ok();
        }

        /// <summary>
        /// Refreshes access & refresh tokens using a valid refresh token.
        /// POST api/v1/auth/refresh
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var (entity, error) = await _loginService.RefreshTokenAsync(request, User);
            if (error is not null) return Unauthorized(error);
            return Ok(entity);
        }

        /// <summary>
        /// Logs out a user and revokes the refresh token.
        /// POST api/v1/auth/logout
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var error = await _logoutService.LogoutAsync(request, User);
            if (error is not null) return BadRequest(error);
            return Ok();
        }

        #endregion 
    }
}

