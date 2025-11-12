
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Issues.DTOs;
using ScaleTrackAPI.Domain.Enums;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Application.Features.Issues.BusinessRules.IssueBusinessRules
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
                return AppError.Validation(ErrorMessages.Get("Issue:InvalidIssueStatus", statusIndex));
            return null;
        }
    }
}
