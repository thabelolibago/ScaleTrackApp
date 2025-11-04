using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Repositories.Implementations;

namespace ScaleTrackAPI.Extensions
{
    public static class RepositoryServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IIssueRepository, IssueRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IIssueTagRepository, IssueTagRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuditTrailRepository, AuditTrailRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();

            return services;
        }
    }
}
