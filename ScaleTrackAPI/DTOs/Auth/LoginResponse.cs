using ScaleTrackAPI.DTOs.User;

namespace ScaleTrackAPI.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public UserResponse User { get; set; } = null!;
    }
}
