using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Shared.Extensions;

namespace ScaleTrackAPI.Shared.Helpers
{
    public class AuditHelper
    {
        private readonly AppDbContext _context;

        public AuditHelper(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Records an audit trail. Can use either a logged-in user (ClaimsPrincipal) or an email for unauthenticated actions.
        /// </summary>
        public async Task RecordAuditAsync<T>(
            string action,
            int entityId,
            T? oldValue,
            T? newValue,
            string entityName,
            object changedBy // ClaimsPrincipal OR string email
        )
        {
            int? userId = null;
            User? changedUser = null;

            switch (changedBy)
            {
                case ClaimsPrincipal principal:
                    userId = principal.GetUserId();
                    break;
                case string email:
                    changedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    userId = changedUser?.Id;
                    break;
            }

            var audit = new AuditTrail
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                ChangedBy = userId,
                ChangedByUser = changedUser,
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


