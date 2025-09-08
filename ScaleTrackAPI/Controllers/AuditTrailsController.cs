using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScaleTrackAPI.DTOs.AuditTrail;
using ScaleTrackAPI.Services;

namespace ScaleTrackAPI.Controllers
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
            var list = await _service.GetAllAuditTrails();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Developer")]
        public async Task<ActionResult<AuditTrailResponse>> GetById(int id)
        {
            var audit = await _service.GetById(id);
            return audit == null ? NotFound() : Ok(audit);
        }
    }
}

