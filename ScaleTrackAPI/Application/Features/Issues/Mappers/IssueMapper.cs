
using ScaleTrackAPI.Application.Features.Issues.DTOs;
using ScaleTrackAPI.Domain.Entities;
using ScaleTrackAPI.Domain.Enums;

namespace ScaleTrackAPI.Application.Features.Issues.Mappers.IssueMapper
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
            UpdatedAt = issue.UpdatedAt,
            CreatedById = issue.CreatedById ?? 0,
            CreatedByName = issue.CreatedBy != null
        ? $"{issue.CreatedBy.FirstName} {issue.CreatedBy.LastName}"
        : "Unknown",
            CreatedByEmail = issue.CreatedBy?.Email ?? "N/A"
        };

        public static Issue ToModel(IssueRequest request, int createdById) => new Issue
        {
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Priority = request.Priority,
            Status = IssueStatus.Open,
            CreatedAt = DateTime.UtcNow,
            CreatedById = createdById
        };
    }
}
