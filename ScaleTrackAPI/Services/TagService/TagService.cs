using ScaleTrackAPI.DTOs.Tag;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Database;
using System.Security.Claims;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Services.TagService
{
    public class TagService : TransactionalServiceBase
    {
        private readonly ITagRepository _repo;
        private readonly TagBusinessRules _businessRules;
        private readonly TagAuditTrail _auditHelper;

        public TagService(
            AppDbContext context,
            ITagRepository repo,
            TagBusinessRules businessRules,
            TagAuditTrail auditHelper
        ) : base(context)
        {
            _repo = repo;
            _businessRules = businessRules;
            _auditHelper = auditHelper;
        }

        public async Task<List<TagResponse>> GetAllTags()
        {
            var tags = await _repo.GetAll();
            return tags.Select(TagMapper.ToResponse).ToList();
        }

        public async Task<(TagResponse? Response, AppError? Error, string? Message)> GetByIdWithMessage(int id)
        {
            var tag = await _repo.GetById(id);
            if (tag == null)
                return (null, AppError.NotFound(ErrorMessages.Get("Tag:TagNotFound", id)), null);

            return (TagMapper.ToResponse(tag), null, null);
        }

        public async Task<(TagResponse? Response, AppError? Error, string? Message)> CreateTag(
            TagRequest request,
            ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(TagResponse? Response, AppError? Error, string? Message)>(async () =>
            {
                var validation = await _businessRules.ValidateCreateAsync(request);
                if (!validation.IsValid)
                    return (null, validation.Error, null);

                var tag = TagMapper.ToModel(request);
                var created = await _repo.Add(tag);

                if (created == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("General:UnexpectedError")), null);

                await _auditHelper.RecordCreate(created, userClaims);

                var successMessage = SuccessMessages.Get("Tag:TagCreated");
                return (TagMapper.ToResponse(created), null, successMessage);
            });
        }

        public async Task<(AppError? Error, string? Message)> DeleteTag(
            int id,
            ClaimsPrincipal userClaims)
        {
            return await ExecuteInTransactionAsync<(AppError? Error, string? Message)>(async () =>
            {
                var validation = await _businessRules.ValidateDeleteAsync(id);
                if (!validation.Exists)
                    return (validation.Error, null);

                var tag = await _repo.GetById(id);
                await _repo.Delete(tag);

                await _auditHelper.RecordDelete(tag, userClaims);

                var successMessage = SuccessMessages.Get("Tag:TagDeleted");
                return (null, successMessage);
            });
        }
    }
}


