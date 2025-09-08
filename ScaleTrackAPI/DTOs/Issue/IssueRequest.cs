namespace ScaleTrackAPI.DTOs.Issue
{
    public class IssueRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Type { get; set; } = null!;
        public IssuePriority Priority { get; set; } = IssuePriority.Medium;
    }
}
