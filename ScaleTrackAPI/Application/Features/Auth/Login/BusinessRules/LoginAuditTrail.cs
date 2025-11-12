using System.Security.Claims;
using System.Text.Json;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Shared.Extensions;

namespace ScaleTrackAPI.Application.Features.Auth.Login.BusinessRules
{
    public class LoginAuditTrail
    {
        private readonly AppDbContext _context;

        private const string EntityNameConst = "User";
        private const string ActionLogin = "Login";
        private const string ActionRefreshToken = "RefreshToken";

        public LoginAuditTrail(AppDbContext context)
        {
            _context = context;
        }

        public Task RecordLogin(User user, ClaimsPrincipal actor) =>
            RecordAsync(user, actor, ActionLogin);

        public Task RecordTokenRefresh(User user, ClaimsPrincipal actor) =>
            RecordAsync(user, actor, ActionRefreshToken);

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
                EntityName = EntityNameConst,
                EntityId = user.Id,
                ChangedBy = actorId,
                ChangedAt = DateTime.UtcNow,
                Changes = JsonSerializer.Serialize(changesObj, new JsonSerializerOptions
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