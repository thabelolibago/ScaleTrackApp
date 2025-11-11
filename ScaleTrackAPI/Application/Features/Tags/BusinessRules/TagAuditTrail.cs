using System.Security.Claims;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Shared.Helpers;

namespace ScaleTrackAPI.Application.Features.Tags.BusinessRules.TagAuditTrail
{
    public class TagAuditTrail
    {
        private readonly AuditHelper _auditHelper;

        private const string EntityName = "Tag";

        public TagAuditTrail(AuditHelper auditHelper)
        {
            _auditHelper = auditHelper;
        }

        public async Task RecordCreate(Tag tag, ClaimsPrincipal user)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Created",
                tag.Id,
                null,
                tag,
                EntityName,
                user
            );
        }

        public async Task RecordDelete(Tag tag, ClaimsPrincipal user)
        {
            await _auditHelper.RecordAuditAsync
            (
                "Deleted",
                tag.Id,
                tag,
                null,
                EntityName,
                user
            );
        }   
    }
}