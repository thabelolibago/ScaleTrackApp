using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Application.Features.Auth.Login.DTOs
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
