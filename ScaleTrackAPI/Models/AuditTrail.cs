namespace ScaleTrackAPI.Models
{
    public class AuditTrail
    {
        public int Id { get; set; }
        public string EntityName { get; set; } = null!;
        public int EntityId { get; set; }
        public string Action { get; set; } = null!;
        public int ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string Changes { get; set; } = null!;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public User? ChangedByUser { get; set; }
        public User? ApprovedByUser { get; set; }
    }
}
