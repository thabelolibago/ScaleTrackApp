using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.DTOs.Auth
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
