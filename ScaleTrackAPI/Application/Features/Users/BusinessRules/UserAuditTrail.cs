using System.Security.Claims;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Shared.Helpers;


namespace ScaleTrackAPI.Application.Features.Users.BusinessRules.UserAuditTrail
{
    public class UserAuditTrail
    {
        private readonly AuditHelper _auditHelper;

        private const string EntityName = "User";

        public UserAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }

        public async Task RecordCreate(User user, ClaimsPrincipal userClaims)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Created",
                user.Id,
                null,
                user,
                EntityName,
                userClaims
            );
        }

        public async Task RecordUpdate(User oldUser, User updated, ClaimsPrincipal userClaims)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Updated",
                updated.Id,
                oldUser,
                updated,
                EntityName,
                userClaims
            );
        }

        public async Task RecordDelete(User deleted, ClaimsPrincipal userClaims)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Deleted",
                deleted.Id,
                deleted,
                null,
                EntityName,
                userClaims
            );
        }
    }
}