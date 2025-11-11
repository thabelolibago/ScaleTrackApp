using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Application.Features.Auth.DTOs.Password;
using ScaleTrackAPI.Application.Features.Auth.Password.ChangePassword.Services.ChangePasswordService;
using ScaleTrackAPI.Application.Features.Auth.Password.ForgotPassword.Services.ForgotPasswordService;
using ScaleTrackAPI.Application.Features.Auth.Password.ResetPassword.Services.ResetPasswordService;
using System.Security.Claims;


namespace ScaleTrackAPI.Controllers
{
    [ApiController]
    [Route("api/v1/password")]
    public class PasswordController : ControllerBase
    {
        private readonly ForgotPasswordService _forgotPasswordService;
        private readonly ResetPasswordService _resetPasswordService;
        private readonly ChangePasswordService _changePasswordService;

        public PasswordController(
            ForgotPasswordService forgotPasswordService,
            ResetPasswordService resetPasswordService,
            ChangePasswordService changePasswordService)
        {
            _forgotPasswordService = forgotPasswordService;
            _resetPasswordService = resetPasswordService;
            _changePasswordService = changePasswordService;
        }

        /// <summary>
        /// Sends a password reset link to the user's email.
        /// POST api/v1/password/forgot
        /// </summary>
        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _forgotPasswordService.SendResetLinkAsync(request);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Resets the user's password using a valid token.
        /// POST api/v1/password/reset
        /// </summary>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _resetPasswordService.ResetPasswordAsync(request, User);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Changes the logged-in user's password.
        /// POST api/v1/password/change
        /// </summary>
        [HttpPost("change")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return BadRequest("Invalid user ID.");

            var result = await _changePasswordService.ChangePasswordAsync(userId, request, User);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

