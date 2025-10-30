using ScaleTrackAPI.DTOs.IssueTag;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Database;
using System.Security.Claims;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services
{
    public class IssueTagService : TransactionalServiceBase
    {
        private readonly IIssueTagRepository _repo;
        private readonly IIssueRepository _issueRepo;
        private readonly IValidator<IssueTagRequest> _validator;
        private readonly AuditHelper _auditHelper;

        public IssueTagService(
            AppDbContext context,
            IIssueTagRepository repo,
            IIssueRepository issueRepo,
            IValidator<IssueTagRequest> validator,
            AuditHelper auditHelper
        ) : base(context)
        {
            _repo = repo;
            _issueRepo = issueRepo;
            _validator = validator;
            _auditHelper = auditHelper;
        }

        public async Task<List<IssueTagResponse>> GetAllTags(int issueId)
        {
            var tags = await _repo.GetAll(issueId);
            return tags.Select(IssueTagMapper.ToResponse).ToList();
        }

        public async Task<(IssueTagResponse? Response, AppError? Error, string? Message)> AddTag(
            int issueId,
            IssueTagRequest request,
            ClaimsPrincipal userClaims
        )
        {
            return await ExecuteInTransactionAsync<(IssueTagResponse? Response, AppError? Error, string? Message)>(async () =>
            {
                var issue = await _issueRepo.GetById(issueId);
                if (issue == null)
                    return (null, AppError.NotFound(ErrorMessages.Get("Issue:IssueNotFound", issueId)), null);

                var validation = _validator.Validate(request);
                if (!validation.IsValid)
                    return (null, AppError.Validation(string.Join("; ", validation.Errors)), null);

                var existing = await _repo.Get(issueId, request.TagId);
                if (existing != null)
                    return (null, AppError.Conflict(ErrorMessages.Get("Tag:TagAlreadyExists", request.TagId)), null);

                var issueTag = IssueTagMapper.ToModel(issueId, request);
                var created = await _repo.Add(issueTag);
                if (created == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("General:UnexpectedError")), null);

                // 🔹 Record audit trail
                await _auditHelper.RecordAuditAsync(
                    action: "Created",
                    entityId: issueId,          // For composite keys, could also combine IssueId+TagId
                    oldValue: null!,
                    newValue: created,
                    entityName: nameof(IssueTag),
                    user: userClaims
                );

                var successMessage = SuccessMessages.Get("Tag:TagCreated");
                return (IssueTagMapper.ToResponse(created), null, successMessage);
            });
        }

        public async Task<(AppError? Error, string? Message)> RemoveTag(
            int issueId,
            int tagId,
            ClaimsPrincipal userClaims
        )
        {
            return await ExecuteInTransactionAsync<(AppError? Error, string? Message)>(async () =>
            {
                var issueTag = await _repo.Get(issueId, tagId);
                if (issueTag == null)
                    return (AppError.NotFound(ErrorMessages.Get("Tag:TagNotFound", tagId)), null);

                await _repo.Delete(issueTag);

                // 🔹 Record audit trail
                await _auditHelper.RecordAuditAsync(
                    action: "Deleted",
                    entityId: issueId, // or combine IssueId+TagId
                    oldValue: issueTag,
                    newValue: null!,
                    entityName: nameof(IssueTag),
                    user: userClaims
                );

                var successMessage = SuccessMessages.Get("Tag:TagDeleted");
                return (null, successMessage);
            });
        }
    }
}
