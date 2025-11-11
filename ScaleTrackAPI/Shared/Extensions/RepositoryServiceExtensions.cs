
using ScaleTrackAPI.Infrastructure.Repositories.Implementations.AuditTrailRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Implementations.CommentRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Implementations.IssueRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Implementations.IssueTagRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Implementations.Menu;
using ScaleTrackAPI.Infrastructure.Repositories.Implementations.PasswordResetRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Implementations.RefreshTokenRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Implementations.TagRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Implementations.UserRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IAuditTrailRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.ICommentRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IIssueRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IIssueTagRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IMenuRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IPasswordResetRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IRefreshTokenRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.ITagRepository;
using ScaleTrackAPI.Infrastructure.Repositories.Interfaces.IUserRepository;

namespace ScaleTrackAPI.Shared.Extensions
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
