using System.Text.RegularExpressions;
using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Users.DTOs;

namespace ScaleTrackAPI.Application.Features.Auth.Validators
{
    public class RegisterValidator
    {
        public static AppError? Validate(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                return AppError.Validation("First name and last name are required.");

            if (request.Email != request.ConfirmEmail)
                return AppError.Validation(ErrorMessages.Get("Validation:EmailsDoNotMatch"));

            if (request.Password != request.ConfirmPassword)
                return AppError.Validation(ErrorMessages.Get("Validation:PasswordsDoNotMatch"));

            if (request.Password.Length < 8)
                return AppError.Validation(ErrorMessages.Get("Validation:PasswordTooShort"));

            if (!Regex.IsMatch(request.Password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).+$"))
                return AppError.Validation("Password must include uppercase, lowercase, number, and symbol.");

            return null;
        }
    }
}
