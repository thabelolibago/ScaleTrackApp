using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.RegisterUser.BusinessRules
{
    public class RegisterUserAuditTrail
    {
        private readonly AuditHelper _auditHelper;
        private const string EntityName = "User";

        public RegisterUserAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }
        public async Task RecordCreate(User user)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Created",
                user.Id,
                null,
                user,
                EntityName,
                null
            );
        }
    }
}