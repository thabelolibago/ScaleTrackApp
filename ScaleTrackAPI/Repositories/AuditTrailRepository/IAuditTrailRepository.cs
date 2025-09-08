using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Repositories
{
    public interface IAuditTrailRepository
    {
        Task<List<AuditTrail>> GetAll();
        Task<AuditTrail?> GetById(int id);
        Task<AuditTrail> Add(AuditTrail audit);
        Task<AuditTrail?> Update(AuditTrail audit);
        Task Delete(AuditTrail audit);
    }
}
