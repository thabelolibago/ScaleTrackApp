using ScaleTrackAPI.Application.Features.Tags.DTOs;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Application.Features.Tags.Validators
{
    public class TagValidator : IValidator<TagRequest>
    {
        public ValidationResult Validate(TagRequest request)
        {
            var errors = new List<string>();

            if (request == null)
                return ValidationResult.Failure("Tag request cannot be null.");

            if (string.IsNullOrWhiteSpace(request.Name))
                errors.Add("Tag name is required.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
