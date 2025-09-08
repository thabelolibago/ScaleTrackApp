namespace ScaleTrackAPI.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<IssueTag> IssueTags { get; set; } = new List<IssueTag>();
    }
}
