using ScaleTrackAPI.Services.Auth;
using ScaleTrackAPI.Services.CommentService;
using ScaleTrackAPI.Services.IssueService;
using ScaleTrackAPI.Services.IssueTagService;
using ScaleTrackAPI.Services.TagService;
using ScaleTrackAPI.Services.UserService;

namespace ScaleTrackAPI.Extensions
{
    public static class AuditTrailServiceExtensions
    {
        public static IServiceCollection AddAuditTrails(this IServiceCollection services)
        {
            services.AddScoped<AuthAuditTrail>();
            services.AddScoped<CommentAuditTrail>();
            services.AddScoped<IssueAuditTrail>();
            services.AddScoped<IssueTagAuditTrail>();
            services.AddScoped<TagAuditTrail>();
            services.AddScoped<UserAuditTrail>();
            return services;
        }
    }
}
