using System.ComponentModel;

namespace ScaleTrackAPI.Domain.Enums
{
    public enum UserRole
    {
        [Description("Admin")]
        Admin = 0,

        [Description("Developer")]
        Developer = 1,

        [Description("Viewer")]
        Viewer = 2
    }
}
