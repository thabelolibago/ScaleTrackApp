using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Issues.BusinessRules.IssueAuditTrail;
using ScaleTrackAPI.Application.Features.Issues.BusinessRules.IssueBusinessRules;
using ScaleTrackAPI.Application.Features.Issues.DTOs;
using ScaleTrackAPI.Application.Features.Issues.Mappers.IssueMapper;
using ScaleTrackAPI.Application.Messages.SuccessMessages;
using ScaleTrackAPI.Domain.Enums;
using ScaleTrackAPI.Infrastructure.Data;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IIssueRepository;
using ScaleTrackAPI.Infrastructure.Services.Base;

namespace ScaleTrackAPI.Application.Features.Issues.Services.IssueService
{
    public class IssueService : TransactionalServiceBase
    {
        private readonly IIssueRepository _repo;
        private readonly AppDbContext _context;
        private readonly IssueBusinessRules _rules;
        private readonly IssueAuditTrail _auditTrail;

        public IssueService(
            AppDbContext context,
            IIssueRepository repo,
            IssueBusinessRules rules,
            IssueAuditTrail auditTrail
        ) : base(context)
        {
            _repo = repo;
            _rules = rules;
            _auditTrail = auditTrail;
            _context = context;
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
                var validationError = _rules.ValidateRequest(request);
                if (validationError != null)
                    return (null, validationError);

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

                // --- AUDIT ---
                await _auditTrail.RecordCreate(fullIssue, userClaims);

                return (IssueMapper.ToResponse(fullIssue), null);
            });
        }

        public async Task<(IssueResponse? Response, AppError? Error)> UpdateIssue(int id, IssueRequest request, ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(IssueResponse? Response, AppError? Error)>(async () =>
            {
                var validationError = _rules.ValidateRequest(request);
                if (validationError != null)
                    return (null, validationError);

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

                // --- AUDIT ---
                await _auditTrail.RecordUpdate(oldIssue, fullIssue!, userClaims);

                return (IssueMapper.ToResponse(fullIssue), null);
            });
        }

        public async Task<(IssueResponse? Response, AppError? Error, string? Message)> UpdateIssueStatus(int id, int statusIndex, ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(IssueResponse? Response, AppError? Error, string? Message)>(async () =>
            {
                var validationError = _rules.ValidateStatus(statusIndex);
                if (validationError != null)
                    return (null, validationError, null);

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
                await _auditTrail.RecordStatusUpdate(oldIssue, fullIssue!, userClaims);

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

                // --- AUDIT ---
                await _auditTrail.RecordDelete(issue, userClaims);

                return (null, SuccessMessages.Get("Issue:IssueDeleted"));
            });
        }
    }
}
