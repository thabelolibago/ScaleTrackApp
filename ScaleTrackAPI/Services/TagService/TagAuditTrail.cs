using System.Security.Claims;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services.TagService
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