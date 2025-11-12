using System.Security.Claims;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Shared.Extensions;

namespace ScaleTrackAPI.Application.Features.Auth.Logout.BusinessRules
{
    public class LogoutAuditTrail
    {
        private readonly AppDbContext _context;
        
        private const string ActionLogout = "Logout";
        
        public Task RecordLogout(User user, ClaimsPrincipal actor) =>
            RecordAsync(user, actor, ActionLogout);

        private async Task RecordAsync(User user, ClaimsPrincipal actor, string action)
        {
            var actorId = actor.GetUserId() ?? user.Id;
            var actorName = actor.Identity?.Name ?? user.Email;

            var changesObj = new
            {
                ActorId = actorId,
                ActorName = actorName,
                Event = action,
                TargetUserId = user.Id
            };

            var audit = new AuditTrail
            {
                Action = action,
                EntityName = "User",
                EntityId = user.Id,
                ChangedBy = actorId,
                ChangedAt = DateTime.UtcNow,
                Changes = System.Text.Json.JsonSerializer.Serialize(changesObj, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                })
            };

            _context.AuditTrails.Add(audit);
            await _context.SaveChangesAsync();
        }

    }
}