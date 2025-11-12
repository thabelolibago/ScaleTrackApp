using ScaleTrackAPI.Application.Errors.ErrorMessages;
using ScaleTrackAPI.Application.Features.Comments.DTOs;
using ScaleTrackAPI.Shared.Validators;

namespace ScaleTrackAPI.Application.Features.Comments.Validators
{
    public class CommentValidator : IValidator<CommentRequest>
    {
        public ValidationResult Validate(CommentRequest request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                errors.Add(ErrorMessages.Get("Comment:CommentRequestNotNull"));
                return ValidationResult.Failure(errors.ToArray());
            }

            if (string.IsNullOrWhiteSpace(request.Content))
                errors.Add(ErrorMessages.Get("Comment:CommentContentRequired"));

            return errors.Count == 0
                ? ValidationResult.Success()
                : ValidationResult.Failure(errors.ToArray());
        }
    }
}
