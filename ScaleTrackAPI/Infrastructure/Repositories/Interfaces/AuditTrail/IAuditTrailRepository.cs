using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IAuditTrailRepository
{
    public interface IAuditTrailRepository
    {
        Task<List<AuditTrail>> GetAll();
        Task<AuditTrail?> GetById(int id);
        Task<AuditTrail> Add(AuditTrail audit);
        Task<AuditTrail?> Update(AuditTrail audit);
    }
}
