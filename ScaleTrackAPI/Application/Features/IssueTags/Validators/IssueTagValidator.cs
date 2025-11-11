using ScaleTrackAPI.Application.Features.IssueTags.DTOs;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Application.Features.IssueTags.Validators
{
    public class IssueTagValidator : IValidator<IssueTagRequest>
    {
        public ValidationResult Validate(IssueTagRequest request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                errors.Add("IssueTag request cannot be null.");
                return ValidationResult.Failure(errors.ToArray());
            }

            if (request.TagId <= 0)
                errors.Add("A valid TagId is required.");

            return errors.Count == 0 
                ? ValidationResult.Success() 
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
