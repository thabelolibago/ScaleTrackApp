using ScaleTrackAPI.DTOs.AuditTrail;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Mappers
{
    public static class AuditTrailMapper
    {
        public static AuditTrailResponse ToResponse(AuditTrail model) => new AuditTrailResponse
        {
            Id = model.Id,
            EntityName = model.EntityName,
            EntityId = model.EntityId,
            Action = model.Action,
            ChangedBy = model.ChangedBy,
            ChangedAt = model.ChangedAt,
            Changes = model.Changes,
            ApprovedBy = model.ApprovedBy,
            ApprovedAt = model.ApprovedAt,
        };

        public static AuditTrail ToModel(AuditTrailRequest request) => new AuditTrail
        {
            EntityName = request.EntityName,
            EntityId = request.EntityId,
            Action = request.Action,
            ChangedBy = request.ChangedBy,
            ChangedAt = DateTime.UtcNow,
            Changes = request.Changes,
            ApprovedBy = request.ApprovedBy
        };
    }
}

