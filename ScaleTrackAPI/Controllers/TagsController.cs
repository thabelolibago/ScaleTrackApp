using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.DTOs.Tag;
using ScaleTrackAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace ScaleTrackAPI.Controllers
{
    [ApiController]
    [Route("api/v1/tags")]
    [Authorize]
    public class TagsController(TagService service) : ControllerBase
    {
        private readonly TagService _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<ActionResult<List<TagResponse>>> GetAll()
        {
            var tags = await _service.GetAllTags();
            return Ok(tags);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TagResponse>> GetById(int id)
        {
            var tag = await _service.GetById(id);
            return tag == null ? NotFound() : Ok(tag);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<IActionResult> Create([FromBody] TagRequest request)
        {
            var (response, error) = await _service.CreateTag(request);
            if (error != null)
            {
                return error.Code switch
                {
                    "ValidationError" => BadRequest(error),
                    "Conflict" => Conflict(error),
                    _ => BadRequest(error)
                };
            }

            return CreatedAtAction(nameof(GetById), new { id = response!.Id }, response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<IActionResult> Delete(int id)
        {
            var error = await _service.DeleteTag(id);
            if (error != null)
                return NotFound(error);

            return NoContent();
        }
    }
}
