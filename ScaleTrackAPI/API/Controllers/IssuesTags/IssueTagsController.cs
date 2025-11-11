using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Application.Features.IssueTags.DTOs;
using ScaleTrackAPI.Application.Features.IssueTags.Services.IssueTagService;

namespace ScaleTrackAPI.Controllers.IssuesTags.IssueTagsController
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
            var (response, error, message) = await _service.AddTag(issueId, request, User);

            if (error is not null)
                return BadRequest(new { error.Message });

            return CreatedAtAction(nameof(GetAll), new { issueId }, new { Data = response, Message = message });
        }

        [HttpDelete("{tagId:int}")]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<IActionResult> Delete(int issueId, int tagId)
        {
            var (error, message) = await _service.RemoveTag(issueId, tagId, User);

            if (error is not null)
                return BadRequest(new { error.Message });

            return Ok(new { Message = message });
        }

    }
}
