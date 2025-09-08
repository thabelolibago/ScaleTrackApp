using ScaleTrackAPI.DTOs.IssueTag;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Mappers;
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

        public async Task<(IssueTagResponse? Response, AppError? Error)> AddTag(int issueId, IssueTagRequest request)
        {
            var issue = await _issueRepo.GetById(issueId);
            if (issue == null)
                return (null, AppError.NotFound($"Issue with id {issueId} not found."));

            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (null, AppError.Validation(string.Join("; ", validation.Errors)));

            var existing = await _repo.Get(issueId, request.TagId);
            if (existing != null)
                return (null, AppError.Conflict($"Tag {request.TagId} already exists for issue {issueId}."));

            var issueTag = IssueTagMapper.ToModel(issueId, request);
            var created = await _repo.Add(issueTag);

            return (IssueTagMapper.ToResponse(created), null);
        }

        public async Task<AppError?> RemoveTag(int issueId, int tagId)
        {
            var issueTag = await _repo.Get(issueId, tagId);
            if (issueTag == null)
                return AppError.NotFound($"Tag {tagId} not found for issue {issueId}.");

            await _repo.Delete(issueTag);
            return null;
        }
    }
}
