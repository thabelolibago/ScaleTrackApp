
using ScaleTrackAPI.Application.Features.IssueTags.DTOs;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.IssueTags.Mappers.IssueTagMapper
{
    public static class IssueTagMapper
    {
        public static IssueTagResponse ToResponse(IssueTag model) => new IssueTagResponse
        {
            IssueId = model.IssueId,
            TagId = model.TagId
        };

        public static IssueTag ToModel(int issueId, IssueTagRequest request) => new IssueTag
        {
            IssueId = issueId,
            TagId = request.TagId
        };
    }
}
