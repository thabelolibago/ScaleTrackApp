using System.ComponentModel.DataAnnotations;
using ScaleTrackAPI.Domain.Enums;

namespace ScaleTrackAPI.Application.Features.Issues.DTOs
{
    public class IssueDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200), MinLength(3)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(2000), MinLength(1)]
        public string Description { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;

        [Required]
        public string Priority { get; set; } = null!;

        [Required]
        public IssueStatus Status { get; set; } = IssueStatus.Open;

        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
