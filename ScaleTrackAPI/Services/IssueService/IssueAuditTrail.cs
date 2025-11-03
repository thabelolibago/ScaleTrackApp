using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Models;
using System.Security.Claims;

namespace ScaleTrackAPI.Services.IssueService
{
    public class IssueAuditTrail
    {
        private readonly AuditHelper _auditHelper;

        public IssueAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }

        private const string EntityName = "Issue";

        public async Task RecordCreate(Issue issue, ClaimsPrincipal user)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Created",
                issue.Id, 
                null,
                issue,
                EntityName,
                user
            );
        }

        public async Task RecordUpdate(Issue oldIssue, Issue updated, ClaimsPrincipal user)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Updated", 
                updated.Id, 
                oldIssue, 
                updated, 
                EntityName, 
                user
            );
        }

        public async Task RecordDelete(Issue deleted, ClaimsPrincipal user)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Deleted", 
                deleted.Id, 
                deleted, 
                null, 
                EntityName, 
                user
            );
        }

        public async Task RecordStatusUpdate(Issue oldIssue, Issue updated, ClaimsPrincipal user)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Status Updated", 
                updated.Id, 
                oldIssue, 
                updated, 
                EntityName, 
                user
            );
        }
    }
}
