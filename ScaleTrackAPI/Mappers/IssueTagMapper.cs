using ScaleTrackAPI.DTOs.IssueTag;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Mappers
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
