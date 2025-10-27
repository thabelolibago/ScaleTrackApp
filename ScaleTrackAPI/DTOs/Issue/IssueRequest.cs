using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.DTOs.Issue
{
    public class IssueRequest
    {
        [Required]
        [StringLength(200), MinLength(3)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(2000), MinLength(1)]
        public string Description { get; set; } = null!;

        [Required]
        public IssueType Type { get; set; } 

        [Required]
        public IssuePriority Priority { get; set; }
    }
}
