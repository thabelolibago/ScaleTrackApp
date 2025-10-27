using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.DTOs.IssueTag
{
    public class IssueTagRequest
    {
        [Required]
        public int TagId { get; set; }
    }
}
