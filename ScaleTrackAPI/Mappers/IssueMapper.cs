using ScaleTrackAPI.Models;
using ScaleTrackAPI.DTOs.Issue;

namespace ScaleTrackAPI.Mappers
{
    public static class IssueMapper
    {
        public static IssueResponse ToResponse(Issue issue) => new IssueResponse
        {
            Id = issue.Id,
            Title = issue.Title,
            Description = issue.Description,
            Type = issue.Type.ToString(),
            Priority = issue.Priority,
            Status = issue.Status.ToString(),
            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt
        };

        public static Issue ToModel(IssueRequest request) => new Issue
        {
            Title = request.Title,
            Description = request.Description,
            Type = Enum.Parse<IssueType>(request.Type, true),
            Priority = request.Priority,
            Status = IssueStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

    }
}


