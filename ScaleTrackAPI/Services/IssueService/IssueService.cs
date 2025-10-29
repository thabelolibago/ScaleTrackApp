using ScaleTrackAPI.DTOs.Issue;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Database;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ScaleTrackAPI.Services
{
    public class IssueService : TransactionalServiceBase
    {
        private readonly IIssueRepository _repo;
        private readonly IValidator<IssueRequest> _validator;
        private readonly AppDbContext _context;
        private readonly AuditHelper _auditHelper;

        public IssueService(
            AppDbContext context,
            IIssueRepository repo,
            IValidator<IssueRequest> validator,
            AuditHelper auditHelper
        ) : base(context)
        {
            _repo = repo;
            _validator = validator;
            _context = context;
            _auditHelper = auditHelper;
        }

        public async Task<List<IssueResponse>> GetAllIssues()
        {
            var issues = await _repo.GetAll();
            return issues.Select(IssueMapper.ToResponse).ToList();
        }

        public async Task<(IssueResponse? Response, AppError? Error)> GetById(int id)
        {
            var issue = await _repo.GetById(id);
            if (issue == null)
                return (null, AppError.NotFound(ErrorMessages.Get("Issue:IssueNotFound", id)));

            return (IssueMapper.ToResponse(issue), null);
        }

        public async Task<(IssueResponse? Response, AppError? Error)> CreateIssue(IssueRequest request, ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(IssueResponse? Response, AppError? Error)>(async () =>
            {
                var validation = _validator.Validate(request);
                if (!validation.IsValid)
                    return (null, AppError.Validation(string.Join("; ", validation.Errors)));

                var userIdClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                    return (null, AppError.Unauthorized(ErrorMessages.Get("General:Unauthorized")));

                var createdById = int.Parse(userIdClaim);

                var issue = IssueMapper.ToModel(request, createdById);
                var created = await _repo.AddIssue(issue);
                if (created == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("General:UnexpectedError")));

                var fullIssue = await _context.Issues
                    .Include(i => i.CreatedBy)
                    .FirstOrDefaultAsync(i => i.Id == created.Id);

                if (fullIssue == null)
                    return (null, AppError.Unexpected("General:UnexpectedError"));

                var audit = AuditTrailMapper.CreateAudit(
                    action: "Created",
                    entityId: fullIssue.Id,
                    oldValue: null,
                    newValue: fullIssue,
                    user: userClaims
                );
                _context.AuditTrails.Add(audit);
                await _context.SaveChangesAsync();

                return (IssueMapper.ToResponse(fullIssue), null);
            });
        }

        public async Task<(IssueResponse? Response, AppError? Error)> UpdateIssue(int id, IssueRequest request, ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(IssueResponse? Response, AppError? Error)>(async () =>
            {
                var validation = _validator.Validate(request);
                if (!validation.IsValid)
                    return (null, AppError.Validation(string.Join("; ", validation.Errors)));

                var issue = await _repo.GetById(id);
                if (issue == null)
                    return (null, AppError.NotFound(ErrorMessages.Get("Issue:IssueNotFound", id)));

                var oldIssue = issue;

                issue.Title = request.Title;
                issue.Description = request.Description;
                issue.Type = request.Type;
                issue.Priority = request.Priority;
                issue.UpdatedAt = DateTime.UtcNow;

                var updated = await _repo.UpdateIssue(issue);
                if (updated == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("General:UnexpectedError")));

                var fullIssue = await _context.Issues
                    .Include(i => i.CreatedBy)
                    .FirstOrDefaultAsync(i => i.Id == updated.Id);

                var audit = AuditTrailMapper.CreateAudit(
                    action: "Updated",
                    entityId: fullIssue!.Id,
                    oldValue: oldIssue,
                    newValue: fullIssue,
                    user: userClaims
                );
                _context.AuditTrails.Add(audit);
                await _context.SaveChangesAsync();

                return (IssueMapper.ToResponse(fullIssue), null);
            });
        }

        public async Task<(IssueResponse? Response, AppError? Error, string? Message)> UpdateIssueStatus(int id, int statusIndex, ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(IssueResponse? Response, AppError? Error, string? Message)>(async () =>
            {
                if (!Enum.IsDefined(typeof(IssueStatus), statusIndex))
                    return (null, AppError.Validation(ErrorMessages.Get("Issue:InvalidIssueStatus", statusIndex)), null);

                var status = (IssueStatus)statusIndex;

                var issue = await _repo.GetById(id);
                if (issue == null)
                    return (null, AppError.NotFound(ErrorMessages.Get("Issue:IssueNotFound", id)), null);

                var oldIssue = issue;

                issue.Status = status;
                issue.UpdatedAt = DateTime.UtcNow;

                var updated = await _repo.UpdateIssue(issue);
                if (updated == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("General:UnexpectedError")), null);

                var fullIssue = await _context.Issues
                    .Include(i => i.CreatedBy)
                    .FirstOrDefaultAsync(i => i.Id == updated.Id);

                // --- AUDIT ---
                var audit = AuditTrailMapper.CreateAudit(
                    action: "StatusUpdated",
                    entityId: fullIssue!.Id,
                    oldValue: oldIssue,
                    newValue: fullIssue,
                    user: userClaims
                );
                _context.AuditTrails.Add(audit);
                await _context.SaveChangesAsync();

                var successMessage = SuccessMessages.Get("Issue:IssueUpdated");
                return (IssueMapper.ToResponse(fullIssue), null, successMessage);
            });
        }

        public async Task<(AppError? Error, string Message)> DeleteIssue(int id, ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(AppError? Error, string Message)>(async () =>
            {
                var issue = await _repo.GetById(id);
                if (issue == null)
                    return (AppError.NotFound(ErrorMessages.Get("Issue:IssueNotFound", id)), null!);

                await _repo.DeleteIssue(id);

                var audit = AuditTrailMapper.CreateAudit(
                    action: "Deleted",
                    entityId: issue.Id,
                    oldValue: issue,
                    newValue: null,
                    user: userClaims
                );
                _context.AuditTrails.Add(audit);
                await _context.SaveChangesAsync();

                return (null, SuccessMessages.Get("Issue:IssueDeleted"));
            });
        }
    }
}
