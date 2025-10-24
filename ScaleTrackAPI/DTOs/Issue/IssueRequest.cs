namespace ScaleTrackAPI.DTOs.Issue
{
    public class IssueRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IssueType Type { get; set; } 
        public IssuePriority Priority { get; set; } = IssuePriority.Medium;
    }
}
