using ScaleTrackAPI.Application.Features.Auth.BusinessRules.AuthAuditTrail;
using ScaleTrackAPI.Application.Features.Auth.Password.Shared.PasswordAuditTrail;
using ScaleTrackAPI.Application.Features.Comments.BusinessRules.CommentAuditTrail;
using ScaleTrackAPI.Application.Features.Issues.BusinessRules.IssueAuditTrail;
using ScaleTrackAPI.Application.Features.IssueTags.BusinessRules.IssueTagAuditTrail;
using ScaleTrackAPI.Application.Features.Profile.EditProfile.BusinessRules.EditProfileAuditTrail;
using ScaleTrackAPI.Application.Features.Tags.BusinessRules.TagAuditTrail;
using ScaleTrackAPI.Application.Features.Users.BusinessRules.UserAuditTrail;

namespace ScaleTrackAPI.Shared.Extensions
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
            services.AddScoped<PasswordAuditTrail>();
            services.AddScoped<EditProfileAuditTrail>();
            return services;
        }
    }
}
