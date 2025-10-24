using ScaleTrackAPI.DTOs.IssueTag;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Repositories;

namespace ScaleTrackAPI.Services
{
    public class IssueTagService(IIssueTagRepository repo, IIssueRepository issueRepo, IValidator<IssueTagRequest> validator)
    {
        private readonly IIssueTagRepository _repo = repo;
        private readonly IIssueRepository _issueRepo = issueRepo;
        private readonly IValidator<IssueTagRequest> _validator = validator;

        public async Task<List<IssueTagResponse>> GetAllTags(int issueId)
        {
            var tags = await _repo.GetAll(issueId);
            return tags.Select(IssueTagMapper.ToResponse).ToList();
        }

        public async Task<(IssueTagResponse? Response, AppError? Error, string? Message)> AddTag(int issueId, IssueTagRequest request)
        {
            var issue = await _issueRepo.GetById(issueId);
            if (issue == null)
                return (null, AppError.NotFound(ErrorMessages.Get("IssueNotFound", issueId)), null);

            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (null, AppError.Validation(string.Join("; ", validation.Errors)), null);

            var existing = await _repo.Get(issueId, request.TagId);
            if (existing != null)
                return (null, AppError.Conflict(ErrorMessages.Get("TagAlreadyExists", request.TagId)), null);

            var issueTag = IssueTagMapper.ToModel(issueId, request);
            var created = await _repo.Add(issueTag);
            if (created == null)
                return (null, AppError.Unexpected(ErrorMessages.Get("UnexpectedError")), null);

            var successMessage = SuccessMessages.Get("TagCreated");
            return (IssueTagMapper.ToResponse(created), null, successMessage);
        }

        public async Task<(AppError? Error, string? Message)> RemoveTag(int issueId, int tagId)
        {
            var issueTag = await _repo.Get(issueId, tagId);
            if (issueTag == null)
                return (AppError.NotFound(ErrorMessages.Get("TagNotFound", tagId)), null);

            await _repo.Delete(issueTag);
            var successMessage = SuccessMessages.Get("TagDeleted");
            return (null, successMessage);
        }

    }
}
