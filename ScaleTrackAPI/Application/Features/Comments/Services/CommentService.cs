using System.Security.Claims;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Comments.BusinessRules.CommentAuditTrail;
using ScaleTrackAPI.Application.Features.Comments.BusinessRules.CommentBusinessRules;
using ScaleTrackAPI.Application.Features.Comments.DTOs;
using ScaleTrackAPI.Application.Features.Comments.Mappers.CommentMapper;
using ScaleTrackAPI.Application.Messages.SuccessMessages;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.ICommentRepository;
using ScaleTrackAPI.Infrastructure.Services.Base;

namespace ScaleTrackAPI.Application.Features.Comments.Services.CommentService
{
    public class CommentService : TransactionalServiceBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly CommentBusinessRules _businessRules;
        private readonly CommentAuditTrail _auditHelper;

        public CommentService(
            AppDbContext context,
            ICommentRepository commentRepo,
            CommentBusinessRules businessRules,
            CommentAuditTrail auditHelper
        ) : base(context)
        {
            _commentRepo = commentRepo;
            _businessRules = businessRules;
            _auditHelper = auditHelper;
        }

        public async Task<List<CommentResponse>> GetCommentsByIssueIdAsync(int issueId)
        {
            var comments = await _commentRepo.GetAll(issueId);
            var responses = new List<CommentResponse>();

            foreach (var comment in comments)
            {
                var userFullName = await _businessRules.GetUserFullNameAsync(comment.UserId);
                responses.Add(CommentMapper.ToResponse(comment, userFullName));
            }

            return responses;
        }

        public async Task<(CommentResponse? Response, AppError? Error)> GetById(int issueId, int id)
        {
            var comment = await _commentRepo.GetById(issueId, id);
            if (comment == null)
                return (null, AppError.NotFound(ErrorMessages.Get("Comment:CommentNotFound", id)));

            var userFullName = await _businessRules.GetUserFullNameAsync(comment.UserId);
            var response = CommentMapper.ToResponse(comment, userFullName);

            return (response, null);
        }

        public async Task<(CommentResponse? Response, AppError? Error)> AddCommentAsync(
            int issueId,
            CommentRequest request,
            ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(CommentResponse? Response, AppError? Error)>(async () =>
            {
                var validation = await _businessRules.ValidateCreateAsync(issueId, request, userClaims);
                if (!validation.IsValid)
                    return (null, validation.Error);

                var comment = CommentMapper.ToModel(issueId, validation.UserId, request);
                var created = await _commentRepo.Add(comment);
                if (created == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("General:UnexpectedError")));

                await _auditHelper.RecordCreate(created, userClaims);

                var userFullName = await _businessRules.GetUserFullNameAsync(validation.UserId);
                var response = CommentMapper.ToResponse(created, userFullName);

                return (response, null);
            });
        }

        public async Task<(AppError? Error, string? Message)> DeleteCommentAsync(
            int issueId,
            int id,
            ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(AppError? Error, string? Message)>(async () =>
            {
                var comment = await _commentRepo.GetById(issueId, id);
                if (comment == null)
                    return (AppError.NotFound(ErrorMessages.Get("Comment:CommentNotFound", id)), null);

                var validation = await _businessRules.ValidateDeleteAsync(comment, userClaims);
                if (!validation.IsAuthorized)
                    return (validation.Error, null);

                await _commentRepo.Delete(comment);
                await _auditHelper.RecordDelete(comment, userClaims);

                return (null, SuccessMessages.Get("Comment:CommentDeleted"));
            });
        }
    }
}
