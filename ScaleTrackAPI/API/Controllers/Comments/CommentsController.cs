using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Application.Features.Comments.DTOs;
using ScaleTrackAPI.Application.Features.Comments.Services.CommentService;

namespace ScaleTrackAPI.Controllers.Comments.CommentsController
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
            var (comment, error) = await _service.GetById(issueId, id);
            if (error is not null) return NotFound(new { error.Message });
            return Ok(comment);
        }

        [HttpPost]
        [Authorize(Roles = "Developer,Admin")]
        public async Task<IActionResult> AddComment(int issueId, [FromBody] CommentRequest request)
        {
            var (response, error) = await _service.AddCommentAsync(issueId, request, User);
            if (error is not null) return BadRequest(new { error.Message });
            return CreatedAtAction(nameof(GetById), new { issueId, id = response!.Id }, response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(int issueId, int id)
        {
            var (error, message) = await _service.DeleteCommentAsync(issueId, id, User);

            if (error is not null)
                return BadRequest(new { error.Message });

            return Ok(new { Message = message });
        }
    }
}
