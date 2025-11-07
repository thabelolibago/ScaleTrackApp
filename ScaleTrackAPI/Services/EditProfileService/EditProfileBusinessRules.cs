using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.Errors;

namespace ScaleTrackAPI.BusinessRules
{
    public class EditProfileBusinessRules
    {
        public AppError? Validate(EditProfileRequest request)
        {
            if (request == null)
                return AppError.Validation("Request cannot be null.");

            bool hasName = !string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName);
            bool hasPicture = !string.IsNullOrWhiteSpace(request.ProfilePictureUrl);
            bool hasPassword = !string.IsNullOrWhiteSpace(request.NewPassword);

            int actionCount = 0;
            if (hasName) actionCount++;
            if (hasPicture) actionCount++;
            if (hasPassword) actionCount++;

            if (actionCount == 0)
                return AppError.Validation("No update fields provided.");

            if (actionCount > 1 && hasPassword)
                return AppError.Validation("Password changes must be done separately via ChangePassword.");

            if (hasName)
            {
                if (!string.IsNullOrWhiteSpace(request.FirstName) && request.FirstName.Length < 2)
                    return AppError.Validation("First name must be at least 2 characters.");

                if (!string.IsNullOrWhiteSpace(request.LastName) && request.LastName.Length < 2)
                    return AppError.Validation("Last name must be at least 2 characters.");
            }

            if (hasPicture)
            {
                if (!Uri.IsWellFormedUriString(request.ProfilePictureUrl, UriKind.Absolute))
                    return AppError.Validation("Profile picture must be a valid URL.");
            }

            return null;
        }
    }
}
