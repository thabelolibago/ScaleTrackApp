using ScaleTrackAPI.DTOs.Tag;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Messages;
using ScaleTrackAPI.Database;

namespace ScaleTrackAPI.Services
{
    public class TagService : TransactionalServiceBase
    {
        private readonly ITagRepository _repo;
        private readonly IValidator<TagRequest> _validator;

        public TagService(
            AppDbContext context,
            ITagRepository repo,
            IValidator<TagRequest> validator
        ) : base(context)
        {
            _repo = repo;
            _validator = validator;
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

        public async Task<(TagResponse? Response, AppError? Error, string? Message)> CreateTag(TagRequest request)
        {
            return await ExecuteInTransactionAsync<(TagResponse? Response, AppError? Error, string? Message)>(async () =>
            {
                var validation = _validator.Validate(request);
                if (!validation.IsValid)
                {
                    var message = validation.Errors.Count > 0
                                  ? string.Join("; ", validation.Errors)
                                  : ErrorMessages.Get("Validation:ValidationFailed");
                    return (null, AppError.Validation(message), null);
                }

                if (await _repo.ExistsByName(request.Name))
                    return (null, AppError.Conflict(ErrorMessages.Get("Tag:TagAlreadyExists", request.Name)), null);

                var tag = TagMapper.ToModel(request);
                var created = await _repo.Add(tag);

                if (created == null)
                    return (null, AppError.Unexpected(ErrorMessages.Get("General:UnexpectedError")), null);

                var successMessage = SuccessMessages.Get("Tag:TagCreated");
                return (TagMapper.ToResponse(created), null, successMessage);
            });
        }

        public async Task<(AppError? Error, string? Message)> DeleteTag(int id)
        {
            return await ExecuteInTransactionAsync<(AppError? Error, string? Message)>(async () =>
            {
                var tag = await _repo.GetById(id);
                if (tag == null)
                    return (AppError.NotFound(ErrorMessages.Get("Tag:TagNotFound", id)), null);

                await _repo.Delete(tag);

                var successMessage = SuccessMessages.Get("Tag:TagDeleted");
                return (null, successMessage);
            });
        }
    }
}

