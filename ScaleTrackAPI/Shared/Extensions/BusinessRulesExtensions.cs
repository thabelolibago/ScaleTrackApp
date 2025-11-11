using ScaleTrackAPI.Application.Features.Auth.BusinessRules.AuthBusinessRules;
using ScaleTrackAPI.Application.Features.Issues.BusinessRules.IssueBusinessRules;
using ScaleTrackAPI.Application.Features.Comments.BusinessRules.CommentBusinessRules;
using ScaleTrackAPI.Application.Features.IssueTags.BusinessRules.IssueTagBusinessRules;
using ScaleTrackAPI.Application.Features.Tags.BusinessRules.TagBusinessRules;
using ScaleTrackAPI.Application.Features.Users.BusinessRules.UserBusinessRules;
using ScaleTrackAPI.Application.Features.Auth.Password.ChangePassword.BusinessRules.ChangePasswordBusinessRules;
using ScaleTrackAPI.Application.Features.Auth.Password.ResetPassword.BusinessRules.ResetPasswordBusinessRules;
using ScaleTrackAPI.Application.Features.Auth.Password.ForgotPassword.BusinessRules.ForgotPasswordBusinessRules;
using ScaleTrackAPI.Application.Features.Profile.EditProfile.BusinessRules.EditProfileBusinessRules;
using ScaleTrackAPI.Application.Features.Auth.RegisterUser.BusinessRules;

namespace ScaleTrackAPI.Shared.Extensions
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
            services.AddScoped<EditProfileBusinessRules>();
            services.AddScoped<RegisterUserBusinessRules>();
            return services;
        }
    }
}
