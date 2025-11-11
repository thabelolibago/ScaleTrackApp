

using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Tags.DTOs;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.ITagRepository;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Application.Features.Tags.BusinessRules.TagBusinessRules
{
    public class TagBusinessRules
    {
        private readonly ITagRepository _tagRepo;
        private readonly IValidator<TagRequest> _validator;

        public TagBusinessRules(ITagRepository tagRepo, IValidator<TagRequest> validator)
        {
            _tagRepo = tagRepo;
            _validator = validator;
        }

        public async Task<(bool IsValid, AppError? Error)> ValidateCreateAsync(TagRequest request)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
            {
                var message = validation.Errors.Count > 0
                    ? string.Join("; ", validation.Errors)
                    : ErrorMessages.Get("Validation:ValidationFailed");

                return (false, AppError.Validation(message));
            }

            if (await _tagRepo.ExistsByName(request.Name))
                return (false, AppError.Conflict(ErrorMessages.Get("Tag:TagAlreadyExists", request.Name)));

            return (true, null);
        }

        public async Task<(bool Exists, AppError? Error)> ValidateDeleteAsync(int id)
        {
            var tag = await _tagRepo.GetById(id);
            if (tag == null)
                return (false, AppError.NotFound(ErrorMessages.Get("Tag:TagNotFound", id)));

            return (true, null);
        }
    }
}
