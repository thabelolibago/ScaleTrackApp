using ScaleTrackAPI.DTOs.User;

namespace ScaleTrackAPI.Helpers
{
    public class UserValidator : IValidator<UserRequest>
    {
        public ValidationResult Validate(UserRequest request)
        {
            var errors = new List<string>();

            if (request == null)
                return ValidationResult.Failure("User request cannot be null.");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                errors.Add("First name is required.");

            if (string.IsNullOrWhiteSpace(request.LastName))
                errors.Add("Last name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required.");

            if (!Enum.TryParse<UserRole>(request.Role, true, out _))
                errors.Add($"Role must be one of: {string.Join(", ", Enum.GetNames<UserRole>())}");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
