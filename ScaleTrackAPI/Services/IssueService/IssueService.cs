using ScaleTrackAPI.DTOs.Issue;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Errors;

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

        public async Task<IssueResponse?> GetById(int id)
        {
            var issue = await _repo.GetById(id);
            return issue == null ? null : IssueMapper.ToResponse(issue);
        }

        public async Task<(IssueResponse? Response, AppError? Error)> CreateIssue(IssueRequest request)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (null, AppError.Validation(string.Join("; ", validation.Errors)));

            var issue = IssueMapper.ToModel(request);
            var created = await _repo.AddIssue(issue);
            return (IssueMapper.ToResponse(created), null);
        }

        public async Task<(IssueResponse? Response, AppError? Error)> UpdateIssue(int id, IssueRequest request)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return (null, AppError.Validation(string.Join("; ", validation.Errors)));

            var issue = await _repo.GetById(id);
            if (issue == null)
                return (null, AppError.NotFound($"Issue with id {id} not found."));

            issue.Title = request.Title;
            issue.Description = request.Description;
            issue.Type = Enum.Parse<IssueType>(request.Type, true);
            issue.Priority = request.Priority;
            issue.UpdatedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateIssue(issue);
            return (updated == null ? null : IssueMapper.ToResponse(updated), null);
        }


        public async Task<(IssueResponse? Response, AppError? Error)> UpdateIssueStatus(int id, IssueStatus status)
        {
            var issue = await _repo.GetById(id);
            if (issue == null)
                return (null, AppError.NotFound($"Issue with id {id} not found."));

            issue.Status = status;
            issue.UpdatedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateIssue(issue);
            return (updated == null ? null : IssueMapper.ToResponse(updated), null);
        }

        public async Task<AppError?> DeleteIssue(int id)
        {
            var issue = await _repo.GetById(id);
            if (issue == null) return AppError.NotFound($"Issue with id {id} not found.");

            await _repo.DeleteIssue(id);
            return null;
        }
    }
}
