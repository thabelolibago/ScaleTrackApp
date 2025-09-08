using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Services;

namespace ScaleTrackAPI.Controllers
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
            var user = await _service.GetById(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var error = await _service.DeleteUser(id);
            if (error != null) return NotFound(error);
            return NoContent();
        }

        [HttpPatch("{id:int}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] string role)
        {
            var error = await _service.UpdateUserRole(id, role);
            if (error != null) return BadRequest(error);
            return NoContent();
        }
    }

}
