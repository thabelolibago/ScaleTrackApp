
using ScaleTrackAPI.Application.Features.Profile.EditProfile.DTOs;
using ScaleTrackAPI.Domain.Entities;

namespace ScaleTrackAPI.Application.Features.Profile.EditProfile.Mappers.EditProfileMapper
{
    public static class EditProfileMapper
    {
        public static bool ApplyProfileChanges(this User user, EditProfileRequest request)
        {
            bool updated = false;

            if (!string.IsNullOrWhiteSpace(request.FirstName) && request.FirstName != user.FirstName)
            {
                user.FirstName = request.FirstName;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.LastName) && request.LastName != user.LastName)
            {
                user.LastName = request.LastName;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.ProfilePictureUrl) &&
                request.ProfilePictureUrl != user.ProfilePictureUrl)
            {
                user.ProfilePictureUrl = request.ProfilePictureUrl;
                updated = true;
            }

            if (!string.IsNullOrWhiteSpace(request.Bio) && request.Bio != user.Bio)
            {
                user.Bio = request.Bio;
                updated = true;
            }

            return updated;
        }

    }
}
