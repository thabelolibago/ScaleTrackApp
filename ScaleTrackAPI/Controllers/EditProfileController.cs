using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Services.UserService;
using ScaleTrackAPI.Errors;
using System.Security.Claims;

namespace ScaleTrackAPI.Controllers
{
    [ApiController]
    [Route("api/v1/profile")]
    [Authorize]
    public class EditProfileController(EditProfileService service) : ControllerBase
    {
        private readonly EditProfileService _service = service;

        /// <summary>
        /// Allows the current user to edit their own profile (name, profile picture, or password).
        /// </summary>
        [HttpPatch]
        public async Task<IActionResult> Edit([FromBody] EditProfileRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { error = "Invalid user credentials." });

            var (response, error) = await _service.EditProfileAsync(userId, request, User);

            if (error is not null)
                return StatusCode(error.StatusCode, new { error.Message });

            return Ok(response);
        }
    }
}
