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
            T oldValue,
            T newValue,
            string entityName,
            ClaimsPrincipal user)
        {
            var userId = user.GetUserId() ?? 0;

            var changes = new
            {
                Old = oldValue,
                New = newValue
            };

            var audit = new AuditTrail
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                ChangedBy = userId,
                Changes = JsonSerializer.Serialize(changes),
                ChangedAt = DateTime.UtcNow
            };

            _context.AuditTrails.Add(audit);
            await _context.SaveChangesAsync();
        }
    }
}
