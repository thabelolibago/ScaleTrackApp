using ScaleTrackAPI.Models;
using ScaleTrackAPI.DTOs.AuditTrail;

namespace ScaleTrackAPI.Mappers
{
    public static class AuditTrailMapper
    {
        public static AuditTrail ToModel(AuditTrailRequest request)
        {
            return new AuditTrail
            {
                EntityName = request.EntityName,
                EntityId = request.EntityId,
                Action = request.Action,
                ChangedBy = request.ChangedBy,
                Changes = request.Changes,
                ApprovedBy = request.ApprovedBy
            };
        }

        public static AuditTrailResponse ToResponse(AuditTrail audit)
        {
            return new AuditTrailResponse
            {
                Id = audit.Id,
                EntityName = audit.EntityName,
                EntityId = audit.EntityId,
                Action = audit.Action,
                ChangedBy = audit.ChangedBy,
                ChangedAt = audit.ChangedAt,
                Changes = audit.Changes,
                ApprovedBy = audit.ApprovedBy,
                ApprovedAt = audit.ApprovedAt
            };
        }
    }
}


