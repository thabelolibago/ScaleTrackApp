using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Application.Features.Comments.DTOs
{
    public class CommentRequest
    {

        [StringLength(1000), MinLength(1),]
        public string Content { get; set; } = null!;
    }
}
