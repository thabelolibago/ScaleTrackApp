using ScaleTrackAPI.DTOs.Issue;
using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;

namespace ScaleTrackAPI.Services.IssueService
{
    public class IssueBusinessRules
    {
        private readonly IValidator<IssueRequest> _validator;

        public IssueBusinessRules(IValidator<IssueRequest> validator)
        {
            _validator = validator;
        }

        public AppError? ValidateRequest(IssueRequest request)
        {
            var validation = _validator.Validate(request);
            if (!validation.IsValid)
                return AppError.Validation(string.Join("; ", validation.Errors));

            return null;
        }

        public AppError? ValidateStatus(int statusIndex)
        {
            if (!Enum.IsDefined(typeof(IssueStatus), statusIndex))
                return AppError.Validation($"Invalid IssueStatus index: {statusIndex}");
            return null;
        }
    }
}
