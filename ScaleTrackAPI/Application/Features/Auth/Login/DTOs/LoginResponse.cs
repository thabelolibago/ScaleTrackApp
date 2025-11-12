using ScaleTrackAPI.Application.Features.Users.DTOs;

namespace ScaleTrackAPI.Application.Features.Auth.Login.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public UserResponse User { get; set; } = null!;
    }
}
