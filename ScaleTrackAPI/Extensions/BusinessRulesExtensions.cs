using ScaleTrackAPI.Services.IssueService;
using ScaleTrackAPI.Services.CommentService;
using ScaleTrackAPI.Services.IssueTagService;
using ScaleTrackAPI.Services.TagService;
using ScaleTrackAPI.Services.UserService;
using ScaleTrackAPI.Services.Auth;
using ScaleTrackAPI.Services;
using ScaleTrackAPI.Services.ResetPassword;
using ScaleTrackAPI.Services.ForgotPassword;

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
            services.AddScoped<ChangePasswordBusinessRules>();
            services.AddScoped<ForgotPasswordBusinessRules>();
            services.AddScoped<ResetPasswordBusinessRules>();

            return services;
        }
    }
}
