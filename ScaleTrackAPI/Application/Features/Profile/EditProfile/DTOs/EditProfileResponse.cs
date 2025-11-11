using ScaleTrackAPI.Application.Features.Users.DTOs;

namespace ScaleTrackAPI.Application.Features.Profile.EditProfile.DTOs
{
    public class EditProfileResponse
    {

        public UserResponse User { get; set; } = null!;
        public string? Message {get; set;}
    }
}