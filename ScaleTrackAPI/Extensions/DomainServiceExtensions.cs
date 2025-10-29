using ScaleTrackAPI.Services;
using ScaleTrackAPI.Services.Auth;

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

            return services;
        }
    }
}
