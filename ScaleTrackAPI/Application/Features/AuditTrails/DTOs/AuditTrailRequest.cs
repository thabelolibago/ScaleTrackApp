using System.ComponentModel.DataAnnotations;

namespace ScaleTrackAPI.Application.Features.AuditTrails.DTOs
{
    public class AuditTrailRequest
    {
        [Required]
        [StringLength(100)]
        public string EntityName { get; set; } = null!;

        [Required]
        public int EntityId { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; } = null!;

        [Required]
        public int ChangedBy { get; set; }

        [Required]
        [StringLength(1000)]
        public string Changes { get; set; } = null!;
        public int? ApprovedBy { get; set; }
    }
}
