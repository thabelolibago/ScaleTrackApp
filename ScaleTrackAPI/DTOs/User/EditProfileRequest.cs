namespace ScaleTrackAPI.DTOs.User
{
    public class EditProfileRequest
    {
        public string? FirstName {get; set;}
        public string? LastName {get; set;}
        public string? Bio { get; set; }
        public string? ProfilePictureUrl {get; set;}
        public string? CurrentPassword {get; set;}
        public string? NewPassword {get; set;}
        public string? ConfirmPassword {get; set;}
    }
}