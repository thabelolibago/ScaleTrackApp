namespace ScaleTrackAPI.Application.Features.Auth.DTOs.Logout
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
