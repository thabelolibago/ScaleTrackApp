using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Application.Features.Auth.RegisterUser.DTOs
{
    public class RegisterUserRequest
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(50)]
        public string LastName { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, EmailAddress]
        public string ConfirmEmail { get; set; } = "";

        [Required, MinLength(6)]
        public string Password { get; set; } = "";

        [Required, MinLength(6)]
        public string ConfirmPassword { get; set; } = "";


        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
