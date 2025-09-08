namespace ScaleTrackAPI.DTOs.Comment
{
    public class CommentRequest
    {
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
    }
}
