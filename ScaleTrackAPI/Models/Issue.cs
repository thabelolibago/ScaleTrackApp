using ScaleTrackAPI.DTOs.Issue;

namespace ScaleTrackAPI.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IssueType Type { get; set; } = IssueType.Bug; 
        public IssuePriority Priority { get; set; } = IssuePriority.Medium;
        public IssueStatus Status { get; set; } = IssueStatus.Open;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<IssueTag> IssueTags { get; set; } = new List<IssueTag>();
    }
}

