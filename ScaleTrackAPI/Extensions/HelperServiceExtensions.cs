using ScaleTrackAPI.Helpers;

namespace ScaleTrackAPI.Extensions
{
    public static class HelperServiceExtensions
    {
        public static IServiceCollection AddHelpers(this IServiceCollection services)
        {
            services.AddSingleton<PasswordHelper>();
            services.AddScoped<AuditHelper>();
            services.AddScoped<EmailHelper>();
            
            return services;
        }
    }
}
