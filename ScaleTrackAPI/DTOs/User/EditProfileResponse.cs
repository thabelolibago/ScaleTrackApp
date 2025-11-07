namespace ScaleTrackAPI.DTOs.User
{
    public class EditProfileResponse
    {

        public UserResponse User { get; set; } = null!;
        public string? Message {get; set;}
    }
}