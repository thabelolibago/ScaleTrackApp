using ScaleTrackAPI.Database;
using ScaleTrackAPI.Models;
using System.Text.Json;
using System.Security.Claims;
using ScaleTrackAPI.Extensions;

namespace ScaleTrackAPI.Helpers
{
    public class AuditHelper
    {
        private readonly AppDbContext _context;

        public AuditHelper(AppDbContext context)
        {
            _context = context;
        }

        public async Task RecordAuditAsync<T>(
            string action,
            int entityId,
            T? oldValue,
            T? newValue,
            string entityName,
            ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            if (userId == null || userId == 0)
                throw new InvalidOperationException("Cannot record audit: user ID is invalid.");

            var audit = new AuditTrail
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                ChangedBy = userId.Value,
                Changes = JsonSerializer.Serialize(
                    new { Old = oldValue, New = newValue },
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                    }),
                ChangedAt = DateTime.UtcNow
            };

            _context.AuditTrails.Add(audit);
            await _context.SaveChangesAsync();

        }
    }
}

