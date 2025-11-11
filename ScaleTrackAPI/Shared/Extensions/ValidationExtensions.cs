using ScaleTrackAPI.Application.Errors.AppError;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Shared.Extensions
{
    public static class ValidationExtensions
    {
        public static AppError? ToAppError<T>(this IValidator<T> validator, T entity)
        {
            var result = validator.Validate(entity);
            if (!result.IsValid)
            {
                return AppError.Validation(string.Join("; ", result.Errors));
            }
            return null;
        }
    }
}
