using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Application.Features.Auth.DTOs.Token
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
