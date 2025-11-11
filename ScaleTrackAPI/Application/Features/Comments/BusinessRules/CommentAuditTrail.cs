using System.Security.Claims;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Shared.Helpers;


namespace ScaleTrackAPI.Application.Features.Comments.BusinessRules.CommentAuditTrail
{
    public class CommentAuditTrail
    {
        private readonly AuditHelper _auditHelper;

        public CommentAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }

        private const string EntityName = "Comment";

        public async Task RecordCreate(Comment comment, ClaimsPrincipal user)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Created",
                comment.Id,
                null,
                comment,
                EntityName,
                user
            );
        }

        public async Task RecordDelete(Comment deleted, ClaimsPrincipal user)
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
    }
}