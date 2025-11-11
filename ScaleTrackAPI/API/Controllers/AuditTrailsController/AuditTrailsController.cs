using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.Application.Features.AuditTrails.DTOs;
using ScaleTrackAPI.Application.Features.AuditTrails.Services.AuditTrailService;

namespace ScaleTrackAPI.Controllers.AuditTrailsController.AuditTrailsController
{
    [ApiController]
    [Route("api/v1/audit-trails")]
    public class AuditTrailsController(AuditTrailService service) : ControllerBase
    {
        private readonly AuditTrailService _service = service;

        [HttpGet]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<ActionResult<IEnumerable<AuditTrailResponse>>> GetAll()
        {
            var audits = await _service.GetAllAuditTrails();
            return Ok(audits);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<ActionResult<AuditTrailResponse>> GetById(int id)
        {
            var (audit, _) = await _service.GetById(id);
            return audit == null ? NotFound() : Ok(audit);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<ActionResult> Delete(int id)
        {
            var error = await _service.DeleteAuditTrail(id);
            return error != null
                ? NotFound(new { message = error.Message })
                : NoContent();
        }

    }
}

