using ScaleTrackAPI.DTOs.Issue;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Messages;

namespace ScaleTrackAPI.Services
{
    public class IssueService(IIssueRepository repo, IValidator<IssueRequest> validator)
    {
        private readonly IIssueRepository _repo = repo;
        private readonly IValidator<IssueRequest> _validator = validator;

        public async Task<List<IssueResponse>> GetAllIssues()
        {
            var issues = await _repo.GetAll();
            return issues.Select(IssueMapper.ToResponse).ToList();
        }

        public async Task<(IssueResponse? Response, AppError? Error)> GetById(int id)
        {
            var issue = await _repo.GetById(id);
            if (issue == null)
                return (null, AppError.NotFound(ErrorMessages.Get("IssueNotFound", id)));

            return (IssueMapper.ToResponse(issue), null);
        }

        public async Task<(IssueResponse? Response, AppError? Error)> CreateIssue(IssueRequest request)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (null, AppError.Validation(string.Join("; ", validation.Errors)));

            var issue = IssueMapper.ToModel(request);
            var created = await _repo.AddIssue(issue);
            if (created == null)
                return (null, AppError.Unexpected(ErrorMessages.Get("UnexpectedError")));

            return (IssueMapper.ToResponse(created), null);
        }

        public async Task<(IssueResponse? Response, AppError? Error)> UpdateIssue(int id, IssueRequest request)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (null, AppError.Validation(string.Join("; ", validation.Errors)));

            var issue = await _repo.GetById(id);
            if (issue == null)
                return (null, AppError.NotFound(ErrorMessages.Get("IssueNotFound", id)));

            issue.Title = request.Title;
            issue.Description = request.Description;
            issue.Type = request.Type;
            issue.Priority = request.Priority;
            issue.UpdatedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateIssue(issue);
            if (updated == null)
                return (null, AppError.Unexpected(ErrorMessages.Get("UnexpectedError")));

            return (IssueMapper.ToResponse(updated), null);
        }

        public async Task<(IssueResponse? Response, AppError? Error, string? Message)> UpdateIssueStatus(int id, int statusIndex)
        {

            if (!Enum.IsDefined(typeof(IssueStatus), statusIndex))
            {
                return (null, AppError.Validation(ErrorMessages.Get("InvalidIssueStatus", statusIndex)), null);
            }

            var status = (IssueStatus)statusIndex;

            var issue = await _repo.GetById(id);
            if (issue == null)
                return (null, AppError.NotFound(ErrorMessages.Get("IssueNotFound", id)), null);

            issue.Status = status;
            issue.UpdatedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateIssue(issue);
            if (updated == null)
                return (null, AppError.Unexpected(ErrorMessages.Get("UnexpectedError")), null);

            var successMessage = SuccessMessages.Get("IssueUpdated");

            return (IssueMapper.ToResponse(updated), null, successMessage);
        }


        public async Task<(AppError? Error, string Message)> DeleteIssue(int id)
        {
            var issue = await _repo.GetById(id);
            if (issue == null)
                return (AppError.NotFound(ErrorMessages.Get("IssueNotFound", id)), null);

            await _repo.DeleteIssue(id);
            return (null, SuccessMessages.Get("IssueDeleted"));
        }
    }
}
