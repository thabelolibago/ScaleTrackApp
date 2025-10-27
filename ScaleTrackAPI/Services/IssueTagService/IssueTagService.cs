using ScaleTrackAPI.DTOs.IssueTag;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Database;

namespace ScaleTrackAPI.Services
{
    public class IssueTagService : TransactionalServiceBase
    {
        private readonly IIssueTagRepository _repo;
        private readonly IIssueRepository _issueRepo;
        private readonly IValidator<IssueTagRequest> _validator;

        public IssueTagService(
            AppDbContext context,
            IIssueTagRepository repo,
            IIssueRepository issueRepo,
            IValidator<IssueTagRequest> validator
        ) : base(context)
        {
            _repo = repo;
            _issueRepo = issueRepo;
            _validator = validator;
        }

        public async Task<List<IssueTagResponse>> GetAllTags(int issueId)
        {
            var tags = await _repo.GetAll(issueId);
            return tags.Select(IssueTagMapper.ToResponse).ToList();
        }

        public async Task<(IssueTagResponse? Response, AppError? Error, string? Message)> AddTag(int issueId, IssueTagRequest request)
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

                var successMessage = SuccessMessages.Get("Tag:TagCreated");
                return (IssueTagMapper.ToResponse(created), null, successMessage);
            });
        }

        public async Task<(AppError? Error, string? Message)> RemoveTag(int issueId, int tagId)
        {
            return await ExecuteInTransactionAsync<(AppError? Error, string? Message)>(async () =>
            {
                var issueTag = await _repo.Get(issueId, tagId);
                if (issueTag == null)
                    return (AppError.NotFound(ErrorMessages.Get("Tag:TagNotFound", tagId)), null);

                await _repo.Delete(issueTag);

                var successMessage = SuccessMessages.Get("Tag:TagDeleted");
                return (null, successMessage);
            });
        }
    }
}
