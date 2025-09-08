namespace ScaleTrackAPI.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Issue? Issue { get; set; }
        public User? User { get; set; }
    }
}
