using ScaleTrackAPI.DTOs.IssueTag;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Database;
using System.Security.Claims;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services.IssueTagService
{
    public class IssueTagService : TransactionalServiceBase
    {
        private readonly IIssueTagRepository _repo;
        private readonly IssueTagBusinessRules _businessRules;
        private readonly IssueTagAuditTrail _auditHelper;

        public IssueTagService(
            AppDbContext context,
            IIssueTagRepository repo,
            IssueTagBusinessRules businessRules,
            IssueTagAuditTrail auditHelper
        ) : base(context)
        {
            _repo = repo;
            _businessRules = businessRules;
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
            ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(IssueTagResponse? Response, AppError? Error, string? Message)>(async () =>
            {
                var validation = await _businessRules.ValidateAddAsync(issueId, request);
                if (!validation.IsValid)
                    return (null, validation.Error, null);

                var issueTag = IssueTagMapper.ToModel(issueId, request);
                var created = await _repo.Add(issueTag);

                if (created == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("General:UnexpectedError")), null);

                await _auditHelper.RecordCreate(created, userClaims);

                var successMessage = SuccessMessages.Get("Tag:TagCreated");
                return (IssueTagMapper.ToResponse(created), null, successMessage);
            });
        }

        public async Task<(AppError? Error, string? Message)> RemoveTag(
            int issueId,
            int tagId,
            ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(AppError? Error, string? Message)>(async () =>
            {
                var validation = await _businessRules.ValidateRemoveAsync(issueId, tagId);
                if (!validation.Exists)
                    return (validation.Error, null);

                var issueTag = await _repo.Get(issueId, tagId);
                await _repo.Delete(issueTag);

                await _auditHelper.RecordDelete(issueTag, userClaims);

                var successMessage = SuccessMessages.Get("Tag:TagDeleted");
                return (null, successMessage);
            });
        }
    }
}
