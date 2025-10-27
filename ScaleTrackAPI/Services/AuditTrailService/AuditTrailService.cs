using ScaleTrackAPI.DTOs.AuditTrail;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Database;

namespace ScaleTrackAPI.Services
{
    public class AuditTrailService : TransactionalServiceBase
    {
        private readonly IAuditTrailRepository _repo;
        private readonly IValidator<AuditTrailRequest> _validator;

        public AuditTrailService(
            AppDbContext context,
            IAuditTrailRepository repo,
            IValidator<AuditTrailRequest> validator
        ) : base(context)
        {
            _repo = repo;
            _validator = validator;
        }

        public async Task<List<AuditTrailResponse>> GetAllAuditTrails()
        {
            var audits = await _repo.GetAll();
            return audits.Select(AuditTrailMapper.ToResponse).ToList();
        }

        public async Task<(AuditTrailResponse? Entity, AppError? Error)> GetById(int id)
        {
            var audit = await _repo.GetById(id);
            if (audit == null)
                return (null, AppError.NotFound(ErrorMessages.Get("Audit:AuditNotFound", id)));

            return (AuditTrailMapper.ToResponse(audit), null);
        }

        public async Task<(AuditTrailResponse? Entity, AppError? Error)> CreateAuditTrail(AuditTrailRequest request)
        {
            return await ExecuteInTransactionAsync<(AuditTrailResponse? Entity, AppError? Error)>(async () =>
            {
                var validation = _validator.Validate(request);
                if (!validation.IsValid)
                {
                    var message = string.Join("; ", validation.Errors);
                    return (null, AppError.Validation(message));
                }

                var audit = AuditTrailMapper.ToModel(request);
                var created = await _repo.Add(audit);

                if (created == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("Audit:AuditCreationFailed")));

                return (AuditTrailMapper.ToResponse(created), null);
            });
        }

        public async Task<(AuditTrailResponse? Entity, AppError? Error)> ApproveAuditTrail(int id, int approverId)
        {
            return await ExecuteInTransactionAsync<(AuditTrailResponse? Entity, AppError? Error)>(async () =>
            {
                var audit = await _repo.GetById(id);
                if (audit == null)
                    return (null, AppError.NotFound(ErrorMessages.Get("Audit:AuditNotFound", id)));

                audit.ApprovedBy = approverId;
                audit.ApprovedAt = DateTime.UtcNow;

                var updated = await _repo.Update(audit);
                if (updated == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("Audit:AuditApprovalFailed")));

                return (AuditTrailMapper.ToResponse(updated), null);
            });
        }

        public async Task<AppError?> DeleteAuditTrail(int id)
        {
            return await ExecuteInTransactionAsync<AppError?>(async () =>
            {
                var audit = await _repo.GetById(id);
                if (audit == null)
                    return AppError.NotFound(ErrorMessages.Get("Audit:AuditNotFound", id));

                await _repo.Delete(audit);
                return null;
            });
        }
    }
}
