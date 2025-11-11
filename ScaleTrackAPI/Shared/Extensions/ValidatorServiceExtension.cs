using Microsoft.AspNetCore.Identity;
using ScaleTrackAPI.Application.Features.AuditTrails.DTOs;
using ScaleTrackAPI.Application.Features.AuditTrails.Validators;
using ScaleTrackAPI.Application.Features.Comments.DTOs;
using ScaleTrackAPI.Application.Features.Comments.Validators;
using ScaleTrackAPI.Application.Features.Issues.DTOs;
using ScaleTrackAPI.Application.Features.Issues.Validators.IssueValidator;
using ScaleTrackAPI.Application.Features.IssueTags.DTOs;
using ScaleTrackAPI.Application.Features.IssueTags.Validators;
using ScaleTrackAPI.Application.Features.Tags.DTOs;
using ScaleTrackAPI.Application.Features.Tags.Validators;
using ScaleTrackAPI.Application.Features.Users.DTOs;
using ScaleTrackAPI.Features.Users.Validators;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Shared.Extensions
{
    public static class ValidatorServiceExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<IssueRequest>, IssueValidator>();
            services.AddSingleton<IValidator<CommentRequest>, CommentValidator>();
            services.AddSingleton<IValidator<IssueTagRequest>, IssueTagValidator>();
            services.AddSingleton<IValidator<TagRequest>, TagValidator>();
            services.AddSingleton<IValidator<UserRequest>, UserValidator>();
            services.AddSingleton<IValidator<AuditTrailRequest>, AuditTrailValidator>();

            return services;
        }
    }
}
