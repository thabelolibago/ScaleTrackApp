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
        /// Records a general audit trail for any entity.
        /// Supports either ClaimsPrincipal (logged-in) or string email (unauthenticated).
        /// </summary>
        public async Task RecordAuditAsync<T>(
            string action,
            int entityId,
            T? oldValue,
            T? newValue,
            string entityName,
            object changedBy // ClaimsPrincipal OR string email OR null
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

        /// <summary>
        /// Convenience overload for user-related actions.
        /// </summary>
        public Task RecordUserActionAsync(User user, string action, ClaimsPrincipal? actor = null)
        {
            object changedBy = actor != null ? actor : (object)user.Email;
            return RecordAuditAsync(action, user.Id, null, user, "User", changedBy);
        }

        // === Optional Convenience Shortcuts ===
        public Task RecordLoginAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "Login", actor);

        public Task RecordLogoutAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "Logout", actor);

        public Task RecordVerifyEmailAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "VerifyEmail", actor);

        public Task RecordRefreshTokenAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "RefreshToken", actor);
    }
}


