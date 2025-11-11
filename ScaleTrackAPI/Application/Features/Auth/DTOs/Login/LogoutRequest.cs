namespace ScaleTrackAPI.Application.Features.Auth.DTOs.Login
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
