using ScaleTrackAPI.Services;
using ScaleTrackAPI.Services.Auth;
using ScaleTrackAPI.Services.CommentService;
using ScaleTrackAPI.Services.IssueService;
using ScaleTrackAPI.Services.IssueTagService;
using ScaleTrackAPI.Services.TagService;
using ScaleTrackAPI.Services.UserService;

namespace ScaleTrackAPI.Extensions
{
    public static class DomainServiceExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IssueService>();
            services.AddScoped<CommentService>();
            services.AddScoped<IssueTagService>();
            services.AddScoped<TagService>();
            services.AddScoped<UserService>();
            services.AddScoped<AuditTrailService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<MenuService>();
            services.AddScoped<PasswordResetService>();
            services.AddScoped<PasswordResetTokenService>();
            services.AddScoped<UserPasswordService>();
            
            return services;
        }
    }
}
