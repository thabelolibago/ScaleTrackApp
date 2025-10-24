using ScaleTrackAPI.DTOs.Issue;
using ScaleTrackAPI.Models;

namespace ScaleTrackAPI.Helpers
{
    public class IssueValidator : IValidator<IssueRequest>
    {
        public ValidationResult Validate(IssueRequest request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                errors.Add("Issue request cannot be null.");
                return ValidationResult.Failure(errors.ToArray());
            }

            if (string.IsNullOrWhiteSpace(request.Title))
                errors.Add("Title is required.");

            if (string.IsNullOrWhiteSpace(request.Description))
                errors.Add("Description is required.");

            if (!Enum.IsDefined(typeof(IssueType), request.Type))
                errors.Add("Invalid Issue Type.");

            if (!Enum.IsDefined(typeof(IssuePriority), request.Priority))
                errors.Add("Invalid Priority.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
