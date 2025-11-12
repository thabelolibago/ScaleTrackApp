namespace ScaleTrackAPI.Application.Features.Auth.VerifyEmail.DTOs
{
    public class EmailVerificationRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
