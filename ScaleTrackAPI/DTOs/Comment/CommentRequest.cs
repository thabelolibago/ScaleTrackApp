using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.DTOs.Comment
{
    public class CommentRequest
    {

        [StringLength(1000), MinLength(1),]
        public string Content { get; set; } = null!;
    }
}
