using System.Text.RegularExpressions;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Application.Features.Auth.RegisterUser
{
    public class RegisterUserValidator : IValidator<RegisterUserRequest>
    {
        public ValidationResult Validate(RegisterUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                return ValidationResult.Failure("First name and last name are required.");

            if (request.Email != request.ConfirmEmail)
                return ValidationResult.Failure(ErrorMessages.Get("Validation:EmailsDoNotMatch"));

            if (request.Password != request.ConfirmPassword)
                return ValidationResult.Failure(ErrorMessages.Get("Validation:PasswordsDoNotMatch"));

            if (request.Password.Length < 8)
                return ValidationResult.Failure(ErrorMessages.Get("Validation:PasswordTooShort"));

            if (!Regex.IsMatch(request.Password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).+$"))
                return ValidationResult.Failure("Password must include uppercase, lowercase, number, and symbol.");

            return ValidationResult.Success();
        }
    }
}
