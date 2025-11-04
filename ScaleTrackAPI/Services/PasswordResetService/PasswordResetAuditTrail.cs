using System.Security.Claims;
using ScaleTrackAPI.Helpers;

namespace ScaleTrackAPI.Services
{
    public class PasswordResetAuditTrail
    {
        private readonly AuditHelper _auditHelper;
        private const string EntityName = "PasswordReset";

        public PasswordResetAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }

        /// <summary>
        /// Audit forgot-password request using email.
        /// </summary>
        public async Task RecordResetLinkSent(string email)
        {
            // Audit anonymously, but store email for traceability
            await _auditHelper.RecordAuditAsync<object>(
                "ResetLinkSent",
                0, // no entity ID yet
                null,
                null,
                EntityName,
                email
            );
        }

        /// <summary>
        /// Audit password reset action.
        /// </summary>
        /// <param name="userId">Id of the user whose password is being reset</param>
        /// <param name="user">Logged-in user performing the reset, may be null</param>
        /// <param name="email">Fallback email to use if user is null</param>
        public async Task RecordReset(int userId, ClaimsPrincipal? user = null, string? email = null)
        {
            // Determine who performed the action
            var changedBy = user?.Identity?.Name ?? email ?? "System";

            await _auditHelper.RecordAuditAsync<object>(
                "PasswordReset",
                userId,
                null,       // old value (optional)
                null,       // new value (optional)
                EntityName,
                changedBy
            );
        }
    }
}
