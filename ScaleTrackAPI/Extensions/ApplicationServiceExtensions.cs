using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ScaleTrackAPI.Database;
using ScaleTrackAPI.Models;
using ScaleTrackAPI.Repositories;
using ScaleTrackAPI.Services;
using ScaleTrackAPI.Services.Auth;
using ScaleTrackAPI.Helpers;
using System.Text;
using ScaleTrackAPI.Validators;
using ScaleTrackAPI.DTOs.Issue;
using ScaleTrackAPI.DTOs.Comment;
using ScaleTrackAPI.DTOs.IssueTag;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.DTOs.Tag;
using ScaleTrackAPI.DTOs.AuditTrail;

namespace ScaleTrackAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")
                                   ?? config.GetConnectionString("DefaultConnection")
                                   ?? "Data Source=Database/ScaleTrack.db";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));

            services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            var jwtKey = config["Jwt:Key"] ?? "ChangeThisSecretInConfig!";
            var jwtIssuer = config["Jwt:Issuer"] ?? "ScaleTrack";
            var jwtAudience = config["Jwt:Audience"] ?? "ScaleTrackClients";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            services.AddScoped<IIssueRepository, IssueRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IIssueTagRepository, IssueTagRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuditTrailRepository, AuditTrailRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddSingleton<IValidator<IssueRequest>, IssueValidator>();
            services.AddSingleton<IValidator<CommentRequest>, CommentValidator>();
            services.AddSingleton<IValidator<IssueTagRequest>, IssueTagValidator>();
            services.AddSingleton<IValidator<TagRequest>, TagValidator>();
            services.AddSingleton<IValidator<UserRequest>, UserValidator>();
            services.AddSingleton<IValidator<AuditTrailRequest>, AuditTrailValidator>();

            services.AddSingleton<PasswordHelper>();

            services.AddScoped<IssueService>();
            services.AddScoped<CommentService>();
            services.AddScoped<IssueTagService>();
            services.AddScoped<TagService>();
            services.AddScoped<UserService>();
            services.AddScoped<AuditTrailService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<MenuService>();

            return services;
        }
    }
}
