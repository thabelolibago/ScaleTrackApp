using System.Security.Claims;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services.UserService
{
    public class EditProfileAuditTrail
    {
        private readonly AuditHelper _auditHelper;
        private const string EntityName = "User";

        public EditProfileAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }

        public async Task RecordUpdate(User oldUser, User newUser, ClaimsPrincipal actor)
        {
            var changes = new
            {
                FirstNameChanged = oldUser.FirstName != newUser.FirstName,
                LastNameChanged = oldUser.LastName != newUser.LastName,
                ProfilePictureChanged = oldUser.ProfilePictureUrl != newUser.ProfilePictureUrl
            };

            await _auditHelper.RecordAuditAsync(
                action: "Updated",
                entityId: newUser.Id,
                oldValue: oldUser,
                newValue: newUser,
                entityName: EntityName,
                changedBy: actor
            );
        }
    }
}

