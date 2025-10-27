using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScaleTrackAPI.Models
{
    public class IssueTag
    {
        [Key, Column(Order = 0)]
        public int IssueId { get; set; }

        [Key, Column(Order = 1)]
        public int TagId { get; set; }

        public Issue? Issue { get; set; }
        public Tag? Tag { get; set; }
    }
}
