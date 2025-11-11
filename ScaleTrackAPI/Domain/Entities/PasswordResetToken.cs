using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScaleTrackAPI.Domain.Entities
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
