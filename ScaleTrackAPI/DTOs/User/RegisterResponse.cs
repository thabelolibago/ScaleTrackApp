namespace ScaleTrackAPI.DTOs.User
{
    public class RegisterResponse
    {
        public UserResponse User { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
