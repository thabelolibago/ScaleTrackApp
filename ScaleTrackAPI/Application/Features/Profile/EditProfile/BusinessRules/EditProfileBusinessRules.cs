using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Profile.EditProfile.DTOs;

namespace ScaleTrackAPI.Application.Features.Profile.EditProfile.BusinessRules.EditProfileBusinessRules
{
    public class EditProfileBusinessRules
    {
        public AppError? Validate(EditProfileRequest request)
        {
            if (request == null)
                return AppError.Validation(ErrorMessages.Get("Profile:RequestNotNull"));

            bool hasName = !string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName);
            bool hasPicture = !string.IsNullOrWhiteSpace(request.ProfilePictureUrl);
            bool hasPassword = !string.IsNullOrWhiteSpace(request.NewPassword);

            int actionCount = 0;
            if (hasName) actionCount++;
            if (hasPicture) actionCount++;
            if (hasPassword) actionCount++;

            if (actionCount == 0)
                return AppError.Validation(ErrorMessages.Get("Profile:NoFieldsProvided"));

            if (actionCount > 1 && hasPassword)
                return AppError.Validation(ErrorMessages.Get("Profile:PasswordUpdateFailed"));

            if (hasName)
            {
                if (!string.IsNullOrWhiteSpace(request.FirstName) && request.FirstName.Length < 2)
                    return AppError.Validation(ErrorMessages.Get("Profile:FirstNameCharMinLength"));

                if (!string.IsNullOrWhiteSpace(request.LastName) && request.LastName.Length < 2)
                    return AppError.Validation(ErrorMessages.Get("Profile:LastNameCharMinLength"));
            }

            if (hasPicture)
            {
                if (!Uri.IsWellFormedUriString(request.ProfilePictureUrl, UriKind.Absolute))
                    return AppError.Validation(ErrorMessages.Get("Profile:PictureUpdateFailed"));
            }

            return null;
        }
    }
}
