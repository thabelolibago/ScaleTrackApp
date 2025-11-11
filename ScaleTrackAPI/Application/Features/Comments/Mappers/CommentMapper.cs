using ScaleTrackAPI.Application.Features.Comments.DTOs;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Comments.Mappers.CommentMapper
{
    public static class CommentMapper
    {
        public static Comment ToModel(int issueId, int userId, CommentRequest request)
        {
            return new Comment
            {
                IssueId = issueId,
                UserId = userId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static CommentResponse ToResponse(Comment comment, string? userName = null)
        {
            return new CommentResponse
            {
                Id = comment.Id,
                IssueId = comment.IssueId,
                UserId = comment.UserId,
                UserName = userName ?? string.Empty,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }
    }
}
