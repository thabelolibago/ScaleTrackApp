using System.ComponentModel;

namespace ScaleTrackAPI.DTOs.Issue
{
    public enum IssueType
    {
        [Description("Bug")]
        Bug = 0,

        [Description("Feature")]
        Feature = 1,

        [Description("Improvement")]
        Improvement = 2,

        [Description("Task")]
        Task = 3
    }
}