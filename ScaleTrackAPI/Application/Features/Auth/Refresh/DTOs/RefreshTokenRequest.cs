using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Application.Features.Auth.Refresh.DTOs
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
