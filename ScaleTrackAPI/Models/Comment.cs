using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IssueId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Issue? Issue { get; set; }
        public User? User { get; set; }
    }
}
