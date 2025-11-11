using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Users.DTOs;
using ScaleTrackAPI.Domain.Enums;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Features.Users.Validators
{
    public class UserValidator : IValidator<UserRequest>
    {
        public ValidationResult Validate(UserRequest request)
        {
            var errors = new List<string>();

            if (request == null)
                return ValidationResult.Failure(ErrorMessages.Get("User:RequestNull"));

            if (string.IsNullOrWhiteSpace(request.FirstName))
                errors.Add(ErrorMessages.Get("User:FirstNameRequired"));

            if (string.IsNullOrWhiteSpace(request.LastName))
                errors.Add(ErrorMessages.Get("User:LastNameRequired"));

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add(ErrorMessages.Get("User:EmailRequired"));

            if (!Enum.IsDefined(typeof(UserRole), request.Role))
                errors.Add($"Role must be one of: {string.Join(", ", Enum.GetNames<UserRole>())}");


            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
