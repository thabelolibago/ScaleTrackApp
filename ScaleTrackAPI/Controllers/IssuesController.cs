using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Services;
using Microsoft.AspNetCore.Authorization;
using ScaleTrackAPI.DTOs.Issue;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Messages;

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
            var (issue, error) = await _service.GetById(id);
            if (error is not null) return NotFound(new { error.Message });
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
            if (error is not null) return BadRequest(new { error.Message });
            return Ok(response);
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int statusIndex)
        {
            var (response, error, message) = await _service.UpdateIssueStatus(id, statusIndex);

            if (error is not null)
                return BadRequest(new { error.Message });

            return Ok(new { Data = response, Message = message });
        }


        [HttpDelete("{id:int}/delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var (error, message) = await _service.DeleteIssue(id);

            if (error is not null)
                return BadRequest(new { error.Message });

            return Ok(new { Message = message });
        }
    }
}
