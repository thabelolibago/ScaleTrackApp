using System.Security.Claims;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.Shared.AuditTrail
{
    /// <summary>
    /// Centralized audit trail helper for all authentication-related actions:
    /// Login, Logout, Register, VerifyEmail, RefreshToken, etc.
    /// </summary>
    public class AuthAuditTrail
    {
        private readonly AuditHelper _auditHelper;
        private const string EntityName = "User";

        public AuthAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }

        /// <summary>
        /// Records a user login event.
        /// </summary>
        public Task RecordLoginAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "Login", actor);

        /// <summary>
        /// Records a user logout event.
        /// </summary>
        public Task RecordLogoutAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "Logout", actor);

        /// <summary>
        /// Records a user registration event.
        /// </summary>
        public Task RecordRegisterAsync(User user, string? email = null) =>
            RecordUserActionAsync(user, "Register", null, email);

        /// <summary>
        /// Records when a user verifies their email address.
        /// </summary>
        public Task RecordVerifyEmailAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "VerifyEmail", actor);

        /// <summary>
        /// Records when a user refreshes their token.
        /// </summary>
        public Task RecordTokenRefreshAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "RefreshToken", actor);

        /// <summary>
        /// Generic private helper for recording user-related actions.
        /// </summary>
        private async Task RecordUserActionAsync(User user, string action, ClaimsPrincipal? actor = null, string? email = null)
        {
            object changedBy = actor != null ? actor : (object)(email ?? user.Email);

            await _auditHelper.RecordAuditAsync<object>(
                action: action,
                entityId: user.Id,
                oldValue: null,
                newValue: user,
                entityName: EntityName,
                changedBy: changedBy
            );
        }
    }
}
