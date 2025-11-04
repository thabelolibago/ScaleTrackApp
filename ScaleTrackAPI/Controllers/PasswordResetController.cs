using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.DTOs.Auth;
using ScaleTrackAPI.Services;
using System.Threading.Tasks;

namespace ScaleTrackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly PasswordResetService _service;

        public PasswordResetController(PasswordResetService service)
        {
            _service = service;
        }

        /// <summary>
        /// Forgot password - send reset link via email
        /// </summary>
        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _service.ForgotPasswordAsync(request);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Reset password using token
        /// </summary>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _service.ResetPasswordAsync(request, User);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

