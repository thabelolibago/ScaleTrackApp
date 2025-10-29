using ScaleTrackAPI.Models;
using ScaleTrackAPI.DTOs.AuditTrail;
using System.Security.Claims;
using System.Text.Json;
using ScaleTrackAPI.Extensions;

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

        // New helper method for services
        public static AuditTrail CreateAudit<T>(
            string action,
            int entityId,
            T oldValue,
            T newValue,
            ClaimsPrincipal user,
            string? entityName = null)
        {
            var userId = user.GetUserId() ?? 0;

            var changes = new
            {
                Old = oldValue,
                New = newValue
            };

            return new AuditTrail
            {
                EntityName = entityName ?? typeof(T).Name,
                EntityId = entityId,
                Action = action,
                ChangedBy = userId,
                Changes = JsonSerializer.Serialize(changes),
                ChangedAt = DateTime.UtcNow
            };
        }
    }
}


