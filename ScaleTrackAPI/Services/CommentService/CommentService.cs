using ScaleTrackAPI.DTOs.Comment;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Database;
using ScaleTrackAPI.Models;
using System.Security.Claims;

namespace ScaleTrackAPI.Services
{
    public class CommentService : TransactionalServiceBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IIssueRepository _issueRepo;
        private readonly IUserRepository _userRepo;
        private readonly IValidator<CommentRequest> _validator;
        private readonly AuditHelper _auditHelper;

        public CommentService(
            AppDbContext context,
            ICommentRepository commentRepo,
            IIssueRepository issueRepo,
            IUserRepository userRepo,
            IValidator<CommentRequest> validator,
            AuditHelper auditHelper
        ) : base(context)

        {
            _commentRepo = commentRepo;
            _issueRepo = issueRepo;
            _userRepo = userRepo;
            _validator = validator;
            _auditHelper = auditHelper;
        }

        public async Task<List<CommentResponse>> GetCommentsByIssueIdAsync(int issueId)
        {
            var comments = await _commentRepo.GetAll(issueId);
            var responses = new List<CommentResponse>();

            foreach (var comment in comments)
            {
                var user = await _userRepo.GetById(comment.UserId);
                responses.Add(CommentMapper.ToResponse(comment, $"{user?.FirstName} {user?.LastName}".Trim()));
            }

            return responses;
        }

        public async Task<(CommentResponse? Response, AppError? Error)> GetById(int issueId, int id)
        {
            var comment = await _commentRepo.GetById(issueId, id);
            if (comment == null)
                return (null, AppError.NotFound(ErrorMessages.Get("Comment:CommentNotFound", id)));

            var user = await _userRepo.GetById(comment.UserId);
            var response = CommentMapper.ToResponse(comment, $"{user?.FirstName} {user?.LastName}".Trim());

            return (response, null);
        }

        public async Task<(CommentResponse? Response, AppError? Error)> AddCommentAsync(int issueId, CommentRequest request, ClaimsPrincipal userClaims
        )
        {
            return await ExecuteInTransactionAsync<(CommentResponse? Response, AppError? Error)>(async () =>
            {
                if (!int.TryParse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                    return (null, AppError.Unauthorized(ErrorMessages.Get("General:Unauthorized")));

                var issue = await _issueRepo.GetById(issueId);
                if (issue == null)
                    return (null, AppError.NotFound(ErrorMessages.Get("Comment:IssueForCommentNotFound", issueId)));

                var validation = _validator.Validate(request);
                if (!validation.IsValid)
                    return (null, AppError.Validation(string.Join("; ", validation.Errors)));

                var comment = CommentMapper.ToModel(issueId, userId, request);
                var created = await _commentRepo.Add(comment);
                if (created == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("General:UnexpectedError")));

                var user = await _userRepo.GetById(userId);

                await _auditHelper.RecordAuditAsync(
                    action: "Created",
                    entityId: created.Id,
                    oldValue: null,
                    newValue: created,
                    entityName: nameof(Comment),
                    user: userClaims
                );

                var response = CommentMapper.ToResponse(created, $"{user?.FirstName} {user?.LastName}".Trim());
                return (response, null);
            });
        }

        public async Task<(AppError? Error, string? Message)> DeleteCommentAsync(
            int issueId,
            int id,
            ClaimsPrincipal userClaims
        )
        {
            return await ExecuteInTransactionAsync<(AppError? Error, string? Message)>(async () =>
            {
                var comment = await _commentRepo.GetById(issueId, id);
                if (comment == null)
                    return (AppError.NotFound(ErrorMessages.Get("Comment:CommentNotFound", id)), null);

                if (!int.TryParse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                    return (AppError.Unauthorized(ErrorMessages.Get("General:Unauthorized")), null);

                if (comment.UserId != userId)
                    return (AppError.Unauthorized(ErrorMessages.Get("Comment:FailedToDeleteComment")), null);

                await _commentRepo.Delete(comment);

                await _auditHelper.RecordAuditAsync(
                    action: "Deleted",
                    entityId: comment.Id,
                    oldValue: comment,
                    newValue: null,
                    entityName: nameof(Comment),
                    user: userClaims
                );

                return (null, SuccessMessages.Get("Comment:CommentDeleted"));
            });
        }
    }
}