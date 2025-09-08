using ScaleTrackAPI.DTOs.AuditTrail;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Helpers;

namespace ScaleTrackAPI.Services
{
    public class AuditTrailService(IAuditTrailRepository repo, IValidator<AuditTrailRequest> validator)
    {
        private readonly IAuditTrailRepository _repo = repo;
        private readonly IValidator<AuditTrailRequest> _validator = validator;

        public async Task<List<AuditTrailResponse>> GetAllAuditTrails()
        {
            var audits = await _repo.GetAll();
            return audits.Select(AuditTrailMapper.ToResponse).ToList();
        }

        public async Task<AuditTrailResponse?> GetById(int id)
        {
            var audit = await _repo.GetById(id);
            return audit == null ? null : AuditTrailMapper.ToResponse(audit);
        }

        public async Task<(AuditTrailResponse? Entity, AppError? Error)> CreateAuditTrail(AuditTrailRequest request)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (null, AppError.Validation(string.Join("; ", validation.Errors)));

            var audit = AuditTrailMapper.ToModel(request);
            var created = await _repo.Add(audit);
            return (AuditTrailMapper.ToResponse(created), null);
        }

        public async Task<(AuditTrailResponse? Entity, AppError? Error)> ApproveAuditTrail(int id, int approverId)
        {
            var audit = await _repo.GetById(id);
            if (audit == null)
                return (null, AppError.NotFound($"Audit trail {id} not found."));

            audit.ApprovedBy = approverId;
            audit.ApprovedAt = DateTime.UtcNow;

            var updated = await _repo.Update(audit);
            return updated == null ? (null, AppError.NotFound("Update failed.")) 
                                   : (AuditTrailMapper.ToResponse(updated), null);
        }

        public async Task<AppError?> DeleteAuditTrail(int id)
        {
            var audit = await _repo.GetById(id);
            if (audit == null)
                return AppError.NotFound($"Audit trail {id} not found.");

            await _repo.Delete(audit);
            return null;
        }
    }
}
