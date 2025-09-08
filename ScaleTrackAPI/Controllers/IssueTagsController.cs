using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.DTOs.IssueTag;
using ScaleTrackAPI.Services;

namespace ScaleTrackAPI.Controllers
{
    [ApiController]
    [Route("api/v1/issues/{issueId:int}/tags")]
    [Authorize]
    public class IssueTagsController(IssueTagService service) : ControllerBase
    {
        private readonly IssueTagService _service = service;

        [HttpGet]
        [Authorize(Roles = "Viewer,Developer,Admin")]
        public async Task<ActionResult<IEnumerable<IssueTagResponse>>> GetAll(int issueId)
        {
            var tags = await _service.GetAllTags(issueId);
            return Ok(tags);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<IActionResult> Add(int issueId, [FromBody] IssueTagRequest request)
        {
            var (response, error) = await _service.AddTag(issueId, request);
            if (error != null)
                return error.Code == "NotFound" ? NotFound(error) :
                       error.Code == "Conflict" ? Conflict(error) :
                       BadRequest(error);

            return CreatedAtAction(nameof(GetAll), new { issueId }, response);
        }

        [HttpDelete("{tagId:int}")]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<IActionResult> Delete(int issueId, int tagId)
        {
            var error = await _service.RemoveTag(issueId, tagId);
            if (error != null)
                return NotFound(error);

            return NoContent();
        }
    }
}
