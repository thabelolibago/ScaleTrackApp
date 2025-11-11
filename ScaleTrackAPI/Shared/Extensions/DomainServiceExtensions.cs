using ScaleTrackAPI.Application.Features.AuditTrails.Services.AuditTrailService;
using ScaleTrackAPI.Application.Features.Auth.Password.ChangePassword.Services.ChangePasswordService;
using ScaleTrackAPI.Application.Features.Auth.Password.ForgotPassword.Services.ForgotPasswordService;
using ScaleTrackAPI.Application.Features.Auth.Password.ResetPassword.Services.ResetPasswordService;
using ScaleTrackAPI.Application.Features.Auth.Password.Shared.PasswordResetTokenService;
using ScaleTrackAPI.Application.Features.Auth.Password.Shared.UserPasswordService;
using ScaleTrackAPI.Application.Features.Auth.Services;
using ScaleTrackAPI.Application.Features.Auth.Services.AuthService;

using ScaleTrackAPI.Application.Features.Auth.Services.TokenService;
using ScaleTrackAPI.Application.Features.Comments.Services.CommentService;
using ScaleTrackAPI.Application.Features.Issues.Services.IssueService;
using ScaleTrackAPI.Application.Features.IssueTags.Services.IssueTagService;
using ScaleTrackAPI.Application.Features.Menu.Services.MenuService;
using ScaleTrackAPI.Application.Features.Profile.EditProfile.Services.EditProfileService;
using ScaleTrackAPI.Application.Features.RegisterUser;
using ScaleTrackAPI.Application.Features.Tags.Services.TagService;
using ScaleTrackAPI.Application.Features.Users.Services.UserService;

namespace ScaleTrackAPI.Shared.Extensions
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
            services.AddScoped<ChangePasswordService>();
            services.AddScoped<PasswordResetTokenService>();
            services.AddScoped<ForgotPasswordService>();
            services.AddScoped<ResetPasswordService>();
            services.AddScoped<UserPasswordService>();
            services.AddScoped<EditProfileService>();
            services.AddScoped<RegisterUserService>();
            
            return services;
        }
    }
}
