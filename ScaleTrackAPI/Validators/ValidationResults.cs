namespace ScaleTrackAPI.Helpers
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public List<string> Errors { get; }

        private ValidationResult(bool isValid, List<string>? errors = null)
        {
            IsValid = isValid;
            Errors = errors ?? new List<string>();
        }

        public static ValidationResult Success() => new(true);
        public static ValidationResult Failure(params string[] errors) => new(false, errors.ToList());
    }

    public interface IValidator<T>
    {
        ValidationResult Validate(T entity);
    }
}
