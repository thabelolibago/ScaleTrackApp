using ScaleTrackAPI.DTOs.Tag;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Mappers;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;
using ScaleTrackAPI.Messages;

namespace ScaleTrackAPI.Services
{
    public class TagService(ITagRepository repo, IValidator<TagRequest> validator)
    {
        private readonly ITagRepository _repo = repo;
        private readonly IValidator<TagRequest> _validator = validator;

        public async Task<List<TagResponse>> GetAllTags()
        {
            var tags = await _repo.GetAll();
            return tags.Select(TagMapper.ToResponse).ToList();
        }

        public async Task<(TagResponse? Response, AppError? Error, string? Message)> GetByIdWithMessage(int id)
        {
            var tag = await _repo.GetById(id);
            if (tag == null)
                return (null, AppError.NotFound(ErrorMessages.Get("TagNotFound", id)), null);

            return (TagMapper.ToResponse(tag), null, null);
        }

        public async Task<(TagResponse? Response, AppError? Error, string? Message)> CreateTag(TagRequest request)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
            {
                var message = validation.Errors.Count > 0
                              ? string.Join("; ", validation.Errors)
                              : ErrorMessages.Get("ValidationFailed");
                return (null, AppError.Validation(message), null);
            }

            if (await _repo.ExistsByName(request.Name))
                return (null, AppError.Conflict(ErrorMessages.Get("TagAlreadyExists", request.Name)), null);

            var tag = TagMapper.ToModel(request);
            var created = await _repo.Add(tag);

            if (created == null)
                return (null, AppError.Unexpected(ErrorMessages.Get("UnexpectedError")), null);

            var successMessage = SuccessMessages.Get("TagCreated");
            return (TagMapper.ToResponse(created), null, successMessage);
        }

        public async Task<(AppError? Error, string? Message)> DeleteTag(int id)
        {
            var tag = await _repo.GetById(id);
            if (tag == null)
                return (AppError.NotFound(ErrorMessages.Get("TagNotFound", id)), null);

            await _repo.Delete(tag);

            var successMessage = SuccessMessages.Get("TagDeleted");
            return (null, successMessage);
        }
    }
}

