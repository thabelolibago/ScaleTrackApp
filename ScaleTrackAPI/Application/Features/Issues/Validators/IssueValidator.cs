using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Issues.DTOs;
using ScaleTrackAPI.Domain.Enums;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Application.Features.Issues.Validators.IssueValidator
{
    public class IssueValidator : IValidator<IssueRequest>
    {
        public ValidationResult Validate(IssueRequest request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                errors.Add(ErrorMessages.Get("Issue:IssueRequestNotNull"));
                return ValidationResult.Failure(errors.ToArray());
            }

            if (string.IsNullOrWhiteSpace(request.Title))
                errors.Add(ErrorMessages.Get("Issue:IssueTitleRequired"));

            if (string.IsNullOrWhiteSpace(request.Description))
                errors.Add(ErrorMessages.Get("Issue:IssueDescriptionRequired"));

            if (!Enum.IsDefined(typeof(IssueType), request.Type))
                errors.Add(ErrorMessages.Get("Issue:InvalidIssueType"));

            if (!Enum.IsDefined(typeof(IssuePriority), request.Priority))
                errors.Add(ErrorMessages.Get("Issue:InvalidIssuePriority"));

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
