using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.DTOs.Comment
{
    public class CommentRequest
    {
        [Required]
        public int UserId { get; set; }

        [StringLength(1000), MinLength(1),]
        public string Content { get; set; } = null!;
    }
}
