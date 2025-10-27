using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.DTOs.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }

        [Required]
        public int IssueId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(1000), MinLength(1),]
        public string Content { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
