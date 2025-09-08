using ScaleTrackAPI.Models;
using ScaleTrackAPI.DTOs.Comment;

namespace ScaleTrackAPI.Mappers
{
    public static class CommentMapper
    {
        public static CommentResponse ToResponse(Comment comment) => new CommentResponse
        {
            Id = comment.Id,
            IssueId = comment.IssueId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt
        };

        public static Comment ToModel(int issueId, CommentRequest request) => new Comment
        {
            IssueId = issueId,
            UserId = request.UserId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };
    }
}
