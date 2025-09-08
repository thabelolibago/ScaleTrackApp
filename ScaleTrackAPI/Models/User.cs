using Microsoft.AspNetCore.Identity;

namespace ScaleTrackAPI.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Role { get; set; } = "User";

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<AuditTrail> ChangesMade { get; set; } = new List<AuditTrail>();
        public ICollection<AuditTrail> ApprovalsGiven { get; set; } = new List<AuditTrail>();
    }
}

