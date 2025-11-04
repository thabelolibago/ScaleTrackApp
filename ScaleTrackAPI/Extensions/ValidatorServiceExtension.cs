using ScaleTrackAPI.DTOs.Issue;
using ScaleTrackAPI.DTOs.Comment;
using ScaleTrackAPI.DTOs.IssueTag;
using ScaleTrackAPI.DTOs.Tag;
using ScaleTrackAPI.DTOs.User;
using ScaleTrackAPI.DTOs.AuditTrail;
using ScaleTrackAPI.Validators;
using ScaleTrackAPI.Helpers;
using Microsoft.AspNetCore.Identity.Data;

namespace ScaleTrackAPI.Extensions
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
