using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime Expires { get; set; }

        [Required]
        public bool IsRevoked { get; set; }

        [Required]
        public bool IsUsed { get; set; }

        
        public User? User { get; set; }
    }
}
