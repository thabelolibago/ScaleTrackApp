using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScaleTrackAPI.DTOs.Issue;

namespace ScaleTrackAPI.Models
{
    public class Issue
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public IssueType Type { get; set; }

        [Required]
        public IssuePriority Priority { get; set; }

        [Required]
        public IssueStatus Status { get; set; } = IssueStatus.Open;

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public int? CreatedById { get; set; }
        public User? CreatedBy { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<IssueTag> IssueTags { get; set; } = new List<IssueTag>();
    }
}

