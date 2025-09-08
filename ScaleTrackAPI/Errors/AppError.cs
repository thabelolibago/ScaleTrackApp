namespace ScaleTrackAPI.Errors
{
    public class AppError
    {
        public string Code { get; }
        public string ErrorMessage { get; }

        public AppError(string code, string message)
        {
            Code = code;
            ErrorMessage = message;
        }

        public static AppError Validation(string message) => new("ValidationError", message);
        public static AppError NotFound(string message) => new("NotFound", message);
        public static AppError Conflict(string message) => new("Conflict", message);
    }
}
