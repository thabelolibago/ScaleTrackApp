namespace ScaleTrackAPI.DTOs.Issue
{
    public class IssueResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Type { get; set; } = null!;
        public IssuePriority Priority { get; set; }
        public string Status { get; set; } = IssueStatus.Open.ToString();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedByEmail { get; set; }
    }
}
