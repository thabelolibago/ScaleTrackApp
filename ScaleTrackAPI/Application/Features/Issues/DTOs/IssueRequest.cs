using System.ComponentModel.DataAnnotations;
using ScaleTrackAPI.Domain.Enums;

namespace ScaleTrackAPI.Application.Features.Issues.DTOs
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
