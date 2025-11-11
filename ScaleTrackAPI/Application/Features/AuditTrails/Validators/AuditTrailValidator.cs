using ScaleTrackAPI.Application.Features.AuditTrails.DTOs;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Application.Features.AuditTrails.Validators
{
    public class AuditTrailValidator : IValidator<AuditTrailRequest>
    {
        public ValidationResult Validate(AuditTrailRequest request)
        {
            if (request == null)
                return ValidationResult.Failure("AuditTrail request cannot be null.");

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.EntityName))
                errors.Add("EntityName is required.");

            if (request.EntityId <= 0)
                errors.Add("EntityId must be provided.");

            if (string.IsNullOrWhiteSpace(request.Action))
                errors.Add("Action is required.");

            if (request.ChangedBy <= 0)
                errors.Add("ChangedBy (user id) is required.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
