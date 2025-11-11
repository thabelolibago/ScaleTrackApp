
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.IssueTags.DTOs;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IIssueRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IIssueTagRepository;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Application.Features.IssueTags.BusinessRules.IssueTagBusinessRules
{
    public class IssueTagBusinessRules
    {
        private readonly IIssueTagRepository _issueTagRepo;
        private readonly IIssueRepository _issueRepo;
        private readonly IValidator<IssueTagRequest> _validator;

        public IssueTagBusinessRules(
            IIssueTagRepository issueTagRepo,
            IIssueRepository issueRepo,
            IValidator<IssueTagRequest> validator)
        {
            _issueTagRepo = issueTagRepo;
            _issueRepo = issueRepo;
            _validator = validator;
        }

        public async Task<(bool IsValid, AppError? Error)> ValidateAddAsync(int issueId, IssueTagRequest request)
        {
            // Check if issue exists
            var issue = await _issueRepo.GetById(issueId);
            if (issue == null)
                return (false, AppError.NotFound(ErrorMessages.Get("Issue:IssueNotFound", issueId)));

            // Validate request fields
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (false, AppError.Validation(string.Join("; ", validation.Errors)));

            // Check for duplicates
            var existing = await _issueTagRepo.Get(issueId, request.TagId);
            if (existing != null)
                return (false, AppError.Conflict(ErrorMessages.Get("Tag:TagAlreadyExists", request.TagId)));

            return (true, null);
        }

        public async Task<(bool Exists, AppError? Error)> ValidateRemoveAsync(int issueId, int tagId)
        {
            var issueTag = await _issueTagRepo.Get(issueId, tagId);
            if (issueTag == null)
                return (false, AppError.NotFound(ErrorMessages.Get("Tag:TagNotFound", tagId)));

            return (true, null);
        }
    }
}
