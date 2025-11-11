using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.AuditTrails.DTOs;
using ScaleTrackAPI.Application.Features.AuditTrails.Mappers.AuditTrailMapper;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IAuditTrailRepository;
using ScaleTrackAPI.Infrastructure.Services.Base;

namespace ScaleTrackAPI.Application.Features.AuditTrails.Services.AuditTrailService
{
    public class AuditTrailService : TransactionalServiceBase
    {
        private readonly IAuditTrailRepository _repo;

        public AuditTrailService(AppDbContext context, IAuditTrailRepository repo)
            : base(context)
        {
            _repo = repo;
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

        public async Task<AppError?> DeleteAuditTrail(int id)
        {
            return await ExecuteInTransactionAsync<AppError?>(async () =>
            {
                var audit = await _repo.GetById(id);
                if (audit == null)
                    return AppError.NotFound(ErrorMessages.Get("Audit:AuditNotFound", id));

                audit.IsDeleted = true;
                audit.DeletedAt = DateTime.UtcNow;

                await _repo.Update(audit);
                return null;
            });
        }
    }
}

