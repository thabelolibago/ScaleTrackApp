using ScaleTrackAPI.Application.Features.Users.DTOs;

namespace ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs
{
    public class RegisterUserResponse
    {
        public UserResponse User { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public bool VerificationPending { get; set; } = false;
        public DateTime? VerificationExpiresAt { get; set; }
    }
}
