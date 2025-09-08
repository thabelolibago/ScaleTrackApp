using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Services;
using ScaleTrackAPI.Errors;
using Microsoft.AspNetCore.Authorization;
using ScaleTrackAPI.DTOs.Issue;

namespace ScaleTrackAPI.Controllers
{
    [ApiController]
    [Route("api/v1/issues")]
    [Authorize]
    public class IssuesController(IssueService service) : ControllerBase
    {
        private readonly IssueService _service = service;

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<IssueResponse>>> GetAll()
        {
            var issues = await _service.GetAllIssues();
            return Ok(issues);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Developer,Viewer")]
        public async Task<ActionResult<IssueResponse>> GetById(int id)
        {
            var issue = await _service.GetById(id);
            if (issue == null) return NotFound();
            return Ok(issue);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<IActionResult> Create([FromBody] IssueRequest request)
        {
            var (response, error) = await _service.CreateIssue(request);

            if (error != null) return BadRequest(error);

            return CreatedAtAction(nameof(GetById), new { id = response!.Id }, response);
        }

        [HttpPut("{id:int}/update")]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<IActionResult> Update(int id, [FromBody] IssueRequest request)
        {
            var (response, error) = await _service.UpdateIssue(id, request);

            if (error != null)
                return error.Code == "NotFound" ? NotFound(error) : BadRequest(error);

            return Ok(response);
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            if (!Enum.TryParse<IssueStatus>(status, true, out var parsedStatus))
                return BadRequest(new AppError("ValidationError", "Invalid status value."));

            var (response, error) = await _service.UpdateIssueStatus(id, parsedStatus);

            if (error != null) return NotFound(error);

            return Ok(response);
        }

        [HttpDelete("{id:int}/delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var error = await _service.DeleteIssue(id);

            if (error != null) return NotFound(error);

            return NoContent();
        }
    }
}
