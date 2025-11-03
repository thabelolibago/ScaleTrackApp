using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.DTOs.Tag;
using Microsoft.AspNetCore.Authorization;
using ScaleTrackAPI.Services.TagService;

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
            var (tag, error, _) = await _service.GetByIdWithMessage(id);
            if (error is not null) return NotFound(new { error.Message });
            return Ok(tag);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<IActionResult> Create([FromBody] TagRequest request)
        {
            var (response, error, message) = await _service.CreateTag(request, User);

            if (error is not null)
                return BadRequest(new { error.Message });

            return CreatedAtAction(nameof(GetById), new { id = response!.Id }, new { Data = response, Message = message });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<IActionResult> Delete(int id)
        {
            var (error, message) = await _service.DeleteTag(id, User);

            if (error is not null)
                return BadRequest(new { error.Message });

            return Ok(new { Message = message });
        }
    }
}
