using ScaleTrackAPI.Application.Errors.ErrorMessages;
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
                errors.Add(ErrorMessages.Get("Tag:TagRequestNotNull"));
                return ValidationResult.Failure(errors.ToArray());
            }

            if (request.TagId <= 0)
                errors.Add(ErrorMessages.Get("Tag:TagNotFound"));

            return errors.Count == 0 
                ? ValidationResult.Success() 
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
