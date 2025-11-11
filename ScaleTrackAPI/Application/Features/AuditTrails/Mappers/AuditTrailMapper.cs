using System.Security.Claims;
using System.Text.Json;
using ScaleTrackAPI.Application.Features.AuditTrails.DTOs;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Shared.Extensions;


namespace ScaleTrackAPI.Application.Features.AuditTrails.Mappers.AuditTrailMapper
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
                ChangedBy = audit.ChangedBy ?? 0,
                ChangedAt = audit.ChangedAt,
                Changes = audit.Changes,
                ApprovedBy = audit.ApprovedBy,
                ApprovedAt = audit.ApprovedAt
            };
        }

        /// <summary>
        /// Helper method used by services to create an AuditTrail record.
        /// Automatically serializes old/new values with proper JSON options.
        /// </summary>
        public static AuditTrail CreateAudit<T>(
            string action,
            int entityId,
            T? oldValue,
            T? newValue,
            ClaimsPrincipal user,
            string? entityName = null)
        {
            // Safely extract user ID — fallback to 0 if null or unauthenticated
            int userId = user.GetUserId() ?? 0;

            var changes = new
            {
                Old = oldValue,
                New = newValue
            };

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };

            return new AuditTrail
            {
                EntityName = entityName ?? typeof(T).Name,
                EntityId = entityId,
                Action = action,
                ChangedBy = userId,
                Changes = JsonSerializer.Serialize(changes, jsonOptions),
                ChangedAt = DateTime.UtcNow
            };
        }
    }
}



