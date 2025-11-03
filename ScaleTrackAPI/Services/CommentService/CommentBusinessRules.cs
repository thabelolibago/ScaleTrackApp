using ScaleTrackAPI.DTOs.Comment;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Repositories;
using System.Security.Claims;

namespace ScaleTrackAPI.Services.CommentService
{
    public class CommentBusinessRules
    {
        private readonly IIssueRepository _issueRepo;
        private readonly IUserRepository _userRepo;
        private readonly IValidator<CommentRequest> _validator;

        public CommentBusinessRules(
            IIssueRepository issueRepo,
            IUserRepository userRepo,
            IValidator<CommentRequest> validator)
        {
            _issueRepo = issueRepo;
            _userRepo = userRepo;
            _validator = validator;
        }

        public async Task<(bool IsValid, AppError? Error, int UserId)> ValidateCreateAsync(
            int issueId, CommentRequest request, ClaimsPrincipal userClaims)
        {
            // User authentication check
            if (!int.TryParse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return (false, AppError.Unauthorized(ErrorMessages.Get("General:Unauthorized")), 0);

            // Issue existence check
            var issue = await _issueRepo.GetById(issueId);
            if (issue == null)
                return (false, AppError.NotFound(ErrorMessages.Get("Comment:IssueForCommentNotFound", issueId)), userId);

            // Request validation
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (false, AppError.Validation(string.Join("; ", validation.Errors)), userId);

            return (true, null, userId);
        }

        public async Task<(bool IsAuthorized, AppError? Error)> ValidateDeleteAsync(
            Comment comment, ClaimsPrincipal userClaims)
        {
            if (!int.TryParse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return (false, AppError.Unauthorized(ErrorMessages.Get("General:Unauthorized")));

            if (comment.UserId != userId)
                return (false, AppError.Unauthorized(ErrorMessages.Get("Comment:FailedToDeleteComment")));

            return (true, null);
        }

        public async Task<string> GetUserFullNameAsync(int userId)
        {
            var user = await _userRepo.GetById(userId);
            return $"{user?.FirstName} {user?.LastName}".Trim();
        }
    }
}
