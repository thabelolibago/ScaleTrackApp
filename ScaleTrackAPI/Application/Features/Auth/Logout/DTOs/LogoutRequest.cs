namespace ScaleTrackAPI.Application.Features.Auth.Logout.DTOs
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
