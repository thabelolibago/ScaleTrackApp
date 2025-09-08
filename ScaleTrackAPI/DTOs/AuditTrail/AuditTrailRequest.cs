namespace ScaleTrackAPI.DTOs.AuditTrail
{
    public class AuditTrailRequest
    {
        public string EntityName { get; set; } = null!;
        public int EntityId { get; set; }
        public string Action { get; set; } = null!;
        public int ChangedBy { get; set; }
        public string Changes { get; set; } = null!;
        public int? ApprovedBy { get; set; }
    }
}
