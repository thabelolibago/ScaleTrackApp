using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Application.Features.Users.DTOs;
using ScaleTrackAPI.Application.Features.Users.Services.UserService;

namespace ScaleTrackAPI.Controllers.Users.UsersController
{
    [ApiController]
    [Route("api/v1/users")]
    [Authorize]
    public class UsersController(UserService service) : ControllerBase
    {
        private readonly UserService _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponse>> GetById(int id)
        {
            var (user, error) = await _service.GetById(id);
            if (error is not null) return NotFound(new { error.Message });
            return Ok(user);
        }

        

        [HttpPatch("{id:int}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] int roleIndex)
        {
            var (error, message) = await _service.UpdateUserRole(id, roleIndex, User);

            if (error != null)
                return BadRequest(new { error.Message });

            return Ok(new { Message = message });
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var (error, message) = await _service.DeleteUser(id, User);

            if (error != null)
                return BadRequest(new { error.Message });

            return Ok(new { Message = message ?? string.Empty });
        }
    }
}
