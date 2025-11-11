using System.ComponentModel;

namespace ScaleTrackAPI.Domain.Enums
{
    public enum IssuePriority
    {
        [Description("Low")]
        Low = 0,

        [Description("Medium")]
        Medium = 1,

        [Description("High")]
        High = 2,
        [Description("Critical")]
        Critical = 3
    }
}
