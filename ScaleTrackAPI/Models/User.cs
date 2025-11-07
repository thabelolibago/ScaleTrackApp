using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.DTOs.User;

namespace ScaleTrackAPI.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public UserRole Role { get; set; } = UserRole.Viewer;
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public bool IsEmailVerified { get; set; } = false;
        public bool RequiresEmailVerification { get; set; } = true;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<AuditTrail> ChangesMade { get; set; } = new List<AuditTrail>();
        public ICollection<AuditTrail> ApprovalsGiven { get; set; } = new List<AuditTrail>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    }
}

