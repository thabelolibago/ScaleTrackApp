using System.Security.Claims;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services.IssueTagService
{
    public class IssueTagAuditTrail
    {
        private readonly AuditHelper _auditHelper;

        private const string EntityName = "IssueTag";

        public IssueTagAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }

        public async Task RecordCreate(IssueTag issueTag, ClaimsPrincipal user)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Created",
                issueTag.IssueId,
                null,
                issueTag,
                EntityName,
                user
            );
        }

        public async Task RecordDelete(IssueTag deleted, ClaimsPrincipal user)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Deleted",
                deleted.IssueId,
                deleted,
                null,
                EntityName,
                user
            );
        }
    }
}