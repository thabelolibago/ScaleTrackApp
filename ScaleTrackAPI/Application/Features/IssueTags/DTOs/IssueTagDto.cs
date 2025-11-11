using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Application.Features.IssueTags.DTOs
{
    public class IssueTagDto
    {
        [Required]
        public int IssueId { get; set; }

        [Required]
        public int TagId { get; set; }
    }
}
