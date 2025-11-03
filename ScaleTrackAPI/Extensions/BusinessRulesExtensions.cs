using ScaleTrackAPI.Services.IssueService;
using ScaleTrackAPI.Services.CommentService;
using ScaleTrackAPI.Services.IssueTagService;
using ScaleTrackAPI.Services.TagService;
using ScaleTrackAPI.Services.UserService;
using ScaleTrackAPI.Services.Auth;

namespace ScaleTrackAPI.Extensions
{
    public static class BusinessRulesExtensions
    {
        public static IServiceCollection AddBusinessRules(this IServiceCollection services)
        {
            services.AddScoped<AuthBusinessRules>();
            services.AddScoped<IssueBusinessRules>();
            services.AddScoped<CommentBusinessRules>();
            services.AddScoped<IssueTagBusinessRules>();
            services.AddScoped<TagBusinessRules>();
            services.AddScoped<UserBusinessRules>();

            return services;
        }
    }
}
