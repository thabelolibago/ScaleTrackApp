using ScaleTrackAPI.DTOs.Comment;

namespace ScaleTrackAPI.Helpers
{
    public class CommentValidator : IValidator<CommentRequest>
    {
        public ValidationResult Validate(CommentRequest request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                errors.Add("Comment request cannot be null.");
                return ValidationResult.Failure(errors.ToArray());
            }

            if (string.IsNullOrWhiteSpace(request.Content))
                errors.Add("Comment content is required.");

            if (request.UserId <= 0)
                errors.Add("A valid UserId is required.");

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
