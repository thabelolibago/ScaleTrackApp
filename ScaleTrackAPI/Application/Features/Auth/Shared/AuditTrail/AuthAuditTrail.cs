using System.Security.Claims;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.Shared.AuditTrail
{
    /// <summary>
    /// Provides a centralized, domain-specific audit trail for all authentication actions.
    /// 
    /// This class abstracts away low-level audit write operations and exposes
    /// high-level semantic methods (Login, Register, VerifyEmail, etc.).  
    /// This should be the *only* class responsible for writing authentication audit logs.
    /// </summary>
    public class AuthAuditTrail
    {
        private readonly AuditHelper _auditHelper;
        private const string EntityName = "User";

        /// <summary>
        /// Creates a new instance of the AuthAuditTrail.
        /// </summary>
        /// <param name="auditHelper">Infrastructure-level helper used to persist audit records.</param>
        public AuthAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }

        // ----------------------------------------------------------------------
        //  PUBLIC ACTION RECORDERS
        // ----------------------------------------------------------------------

        /// <summary>
        /// Writes an audit entry indicating that the specified user successfully logged in.
        /// </summary>
        /// <param name="user">The user performing the login.</param>
        /// <param name="actor">
        /// Optional: a ClaimsPrincipal representing the authenticated actor.  
        /// If null, the user’s own email is used as the actor.
        /// </param>
        public Task RecordLoginAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "Login", actor);

        /// <summary>
        /// Writes an audit entry indicating that the user logged out.
        /// </summary>
        public Task RecordLogoutAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "Logout", actor);

        /// <summary>
        /// Writes an audit entry indicating a newly-registered user.
        /// Typically called immediately after a successful registration.
        /// </summary>
        /// <param name="user">The newly created user entity.</param>
        /// <param name="email">
        /// Optional: used when no ClaimsPrincipal exists yet (before authentication).
        /// </param>
        public Task RecordRegisterAsync(User user, string? email = null) =>
            RecordUserActionAsync(user, "Register", null, email);

        /// <summary>
        /// Writes an audit entry indicating that the user has verified their email address.
        /// </summary>
        public Task RecordVerifyEmailAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "VerifyEmail", actor);

        /// <summary>
        /// Writes an audit entry indicating that the user refreshed their JWT or session token.
        /// </summary>
        public Task RecordTokenRefreshAsync(User user, ClaimsPrincipal? actor = null) =>
            RecordUserActionAsync(user, "RefreshToken", actor);

        // ----------------------------------------------------------------------
        //  INTERNAL CORE METHOD
        // ----------------------------------------------------------------------

        /// <summary>
        /// Core method used by all auth audit actions.  
        /// 
        /// Ensures:
        /// - Consistent audit formatting.
        /// - Centralized control of "changedBy" resolution.
        /// - Minimal duplication across high-level audit calls.
        /// </summary>
        /// <param name="user">The user entity affected by the action.</param>
        /// <param name="action">The name of the action being recorded (e.g., Login, Register).</param>
        /// <param name="actor">Optional ClaimsPrincipal of the acting identity.</param>
        /// <param name="email">
        /// Optional email override used for unauthenticated actions (e.g., Registration
        /// before login). Ignored if <paramref name="actor"/> is supplied.
        /// </param>
        private async Task RecordUserActionAsync(
            User user,
            string action,
            ClaimsPrincipal? actor = null,
            string? email = null)
        {
            // Determine who performed the action.
            //
            // Priority:
            // 1. ClaimsPrincipal (authenticated actor)
            // 2. Provided email (unauthenticated registration scenario)
            // 3. User.Email (fallback)
            object changedBy = actor ?? (object)(email ?? user.Email);

            // All auth audits write the entire user object as the "new state".
            // oldValue intentionally remains null — these are event logs, not diffs.
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
