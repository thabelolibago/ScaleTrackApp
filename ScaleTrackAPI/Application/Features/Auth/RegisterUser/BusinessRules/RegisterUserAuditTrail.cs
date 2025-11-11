using System.Security.Claims;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.RegisterUser.BusinessRules
{
    public class RegisterUserAuditTrail
    {
        private readonly AuditHelper _auditHelper;
        private const string EntityName = "User";
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
    }
  
}