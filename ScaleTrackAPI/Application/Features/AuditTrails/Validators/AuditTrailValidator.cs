using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.AuditTrails.DTOs;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Application.Features.AuditTrails.Validators
{
    public class AuditTrailValidator : IValidator<AuditTrailRequest>
    {
        public ValidationResult Validate(AuditTrailRequest request)
        {
            if (request == null)
                return ValidationResult.Failure(ErrorMessages.Get("Audit:AuditRequestNotNull"));

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.EntityName))
                errors.Add(ErrorMessages.Get("Audit:EntityNameRequired"));

            if (request.EntityId <= 0)
                errors.Add(ErrorMessages.Get("Audit:EntityIdRequired"));

            if (string.IsNullOrWhiteSpace(request.Action))
                errors.Add(ErrorMessages.Get("Audit:ActionRequired"));

            if (request.ChangedBy <= 0)
                errors.Add(ErrorMessages.Get("Audit:ChangedByRequired"));

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
