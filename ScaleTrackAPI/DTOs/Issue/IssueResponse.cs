namespace ScaleTrackAPI.DTOs.Issue
{
    public class IssueResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Type { get; set; } = null!;
        public IssuePriority Priority { get; set; } = IssuePriority.Medium;
        public string Status { get; set; } = IssueStatus.Open.ToString();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
