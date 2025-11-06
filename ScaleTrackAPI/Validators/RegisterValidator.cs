using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Errors;
using System.Text.RegularExpressions;

namespace ScaleTrackAPI.BusinessRules
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
