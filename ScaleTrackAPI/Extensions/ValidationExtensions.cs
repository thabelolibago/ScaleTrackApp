using ScaleTrackAPI.Errors;
using ScaleTrackAPI.Helpers;

namespace ScaleTrackAPI.Extensions
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
