using System.ComponentModel;

namespace ScaleTrackAPI.DTOs.Issue
{
    public enum IssueStatus
    {
        [Description("Open")]
        Open = 0,

        [Description("In Progress")]
        InProgress = 1,

        [Description("Resolved")]
        Resolved = 2,

        [Description("Closed")]
        Closed = 3
    }
}