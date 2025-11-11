namespace ScaleTrackAPI.Application.Errors.AppError 
{
    /// <summary>
    /// Represents a standardized error object returned from the API.
    /// </summary>
    public class AppError
    {
        public int StatusCode { get; }
        public string Message { get; }

        private AppError(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public static AppError Validation(string message) => new(400, message);
        public static AppError NotFound(string message) => new(404, message);
        public static AppError Conflict(string message) => new(409, message);
        public static AppError Unauthorized(string message) => new(401, message);
        public static AppError Forbidden(string message) => new(403, message);

        /// <summary>
        /// Represents an unexpected (unhandled) server-side error.
        /// </summary>
        public static AppError Unexpected(string message) => new(500, message);

        /// <summary>
        /// General-purpose internal server error.
        /// </summary>
        public static AppError ServerError(string message) => new(500, message);
    }
}
