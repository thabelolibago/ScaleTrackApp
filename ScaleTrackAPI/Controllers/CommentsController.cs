using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.DTOs.Comment;
using ScaleTrackAPI.Services;

namespace ScaleTrackAPI.Controllers
{
    [ApiController]
    [Route("api/v1/issues/{issueId:int}/comments")]
    [Authorize]
    public class CommentsController(CommentService service) : ControllerBase
    {
        private readonly CommentService _service = service;

        [HttpGet]
        [Authorize(Roles = "Viewer,Developer,Admin")]
        public async Task<ActionResult<IEnumerable<CommentResponse>>> GetComments(int issueId)
        {
            var comments = await _service.GetCommentsByIssueIdAsync(issueId);
            return Ok(comments);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<CommentResponse>> GetById(int issueId, int id)
        {
            var comment = await _service.GetById(issueId, id);
            return comment == null ? NotFound() : Ok(comment);
        }

        [HttpPost]
        [Authorize(Roles = "Developer,Admin")]
        public async Task<IActionResult> AddComment(int issueId, [FromBody] CommentRequest request)
        {
            var (response, error) = await _service.AddCommentAsync(issueId, request);
            if (error != null)
                return error.Code == "NotFound" ? NotFound(error) : BadRequest(error);

            return CreatedAtAction(nameof(GetById), new { issueId, id = response!.Id }, response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(int issueId, int id)
        {
            var error = await _service.DeleteCommentAsync(issueId, id);
            if (error != null)
                return NotFound(error);

            return NoContent();
        }
    }
}
