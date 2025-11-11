using System.Security.Claims;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Auth.Password.Shared.PasswordAuditTrail
{
    /// <summary>
    /// Audit trail for all password-related actions.
    /// Shared between Forgot, Reset, and Change password.
    /// </summary>
    public class PasswordAuditTrail
    {
        private readonly AuditHelper _auditHelper;

        public PasswordAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }

        public async Task RecordPasswordAction(
            int userId,
            ClaimsPrincipal? performedBy = null,
            string? email = null,
            string action = "Reset")
        {
            var changedBy = performedBy?.Identity?.Name ?? email ?? "System";

            await _auditHelper.RecordAuditAsync<object>(
                action: $"Password{action}",
                entityId: userId,
                oldValue: null,
                newValue: null,
                entityName: "Password",
                changedBy: changedBy
            );
        }

        public async Task RecordResetLinkSent(string email)
        {
            await _auditHelper.RecordAuditAsync<object>(
                action: "PasswordResetLinkSent",
                entityId: 0,
                oldValue: null,
                newValue: null,
                entityName: "Password",
                changedBy: email
            );
        }
    }
}
